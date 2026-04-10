using Microsoft.EntityFrameworkCore;
using EventHub.Data;
using EventHub.Data.Models;
using EventHub.Services.Interfaces;

namespace EventHub.Services;

public class EventService : IEventService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public EventService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Participants)
            .OrderByDescending(e => e.StartDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Events
            .Include(e => e.Organizer)
            .Include(e => e.Participants)
                .ThenInclude(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Event>> GetEventsByOrganizerAsync(string organizerId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Events
            .Include(e => e.Participants)
            .Where(e => e.OrganizerId == organizerId)
            .OrderByDescending(e => e.StartDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Event> CreateEventAsync(Event eventItem)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        eventItem.CreatedAt = DateTime.UtcNow;
        context.Events.Add(eventItem);
        await context.SaveChangesAsync();

        return eventItem;
    }

    public async Task<Event?> UpdateEventAsync(int id, Event eventItem)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var existingEvent = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (existingEvent == null)
            return null;

        existingEvent.Title = eventItem.Title;
        existingEvent.Description = eventItem.Description;
        existingEvent.StartDate = eventItem.StartDate;
        existingEvent.EndDate = eventItem.EndDate;
        existingEvent.Location = eventItem.Location;
        existingEvent.ImageUrl = eventItem.ImageUrl;
        existingEvent.MaxParticipants = eventItem.MaxParticipants;
        existingEvent.Price = eventItem.Price;
        existingEvent.EventType = eventItem.EventType;

        existingEvent.Status = eventItem.Status;
        existingEvent.ModerationStatus = eventItem.ModerationStatus;
        existingEvent.ModeratedAt = eventItem.ModeratedAt;
        existingEvent.ModeratedById = eventItem.ModeratedById;

        existingEvent.CategoryId = eventItem.CategoryId;
        existingEvent.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existingEvent;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var eventItem = await context.Events
            .Include(e => e.Participants)
            .Include(e => e.Reviews)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
            return false;

        // Сначала удаляем связанных участников
        if (eventItem.Participants.Any())
        {
            context.Participants.RemoveRange(eventItem.Participants);
        }

        // Потом удаляем связанные отзывы
        if (eventItem.Reviews.Any())
        {
            context.Reviews.RemoveRange(eventItem.Reviews);
        }

        

        // И только потом удаляем само мероприятие
        context.Events.Remove(eventItem);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EventExistsAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Events.AnyAsync(e => e.Id == id);
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(int count = 10)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Events
            .Where(e =>
                e.StartDate > DateTime.UtcNow &&
                e.Status == EventStatus.Published &&
                e.ModerationStatus == EventModerationStatus.Approved)
            .OrderBy(e => e.StartDate)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }
}