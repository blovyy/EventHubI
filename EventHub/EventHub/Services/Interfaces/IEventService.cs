using EventHub.Data.Models;

namespace EventHub.Services.Interfaces;

public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task<List<Event>> GetEventsByOrganizerAsync(string organizerId);
    Task<Event> CreateEventAsync(Event eventItem);
    Task<Event?> UpdateEventAsync(int id, Event eventItem);
    Task<bool> DeleteEventAsync(int id);
    Task<bool> EventExistsAsync(int id);
    Task<List<Event>> GetUpcomingEventsAsync(int count = 10);
}