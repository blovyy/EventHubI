using Microsoft.EntityFrameworkCore;
using EventHub.Data;
using EventHub.Data.Models;
using EventHub.Services.Interfaces;

namespace EventHub.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .Include(t => t.Owner)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Ticket?> GetTicketByNumberAsync(string ticketNumber)
    {
        return await _context.Tickets
            .Include(t => t.Event)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
    }

    public async Task<List<Ticket>> GetTicketsByEventAsync(int eventId)
    {
        return await _context.Tickets
            .Where(t => t.EventId == eventId)
            .Include(t => t.Owner)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetTicketsByUserAsync(string userId)
    {
        return await _context.Tickets
            .Where(t => t.OwnerId == userId)
            .Include(t => t.Event)
            .ToListAsync();
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        ticket.TicketNumber = Guid.NewGuid().ToString();
        ticket.PurchaseDate = DateTime.UtcNow;
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<bool> UseTicketAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null || ticket.IsUsed)
            return false;

        ticket.IsUsed = true;
        ticket.UsedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTicketAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
            return false;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }
}