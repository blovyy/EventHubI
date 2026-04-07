using EventHub.Data.Models;

namespace EventHub.Services.Interfaces;

public interface ITicketService
{
    Task<List<Ticket>> GetAllTicketsAsync();
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task<Ticket?> GetTicketByNumberAsync(string ticketNumber);
    Task<List<Ticket>> GetTicketsByEventAsync(int eventId);
    Task<List<Ticket>> GetTicketsByUserAsync(string userId);
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task<bool> UseTicketAsync(int id);
    Task<bool> DeleteTicketAsync(int id);
}