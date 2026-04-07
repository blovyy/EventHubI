using EventHub.Data.Models;

namespace EventHub.Services.Interfaces;

public interface IParticipantService
{
    Task<List<Participant>> GetAllParticipantsAsync();
    Task<Participant?> GetParticipantByIdAsync(int id);
    Task<List<Participant>> GetParticipantsByEventAsync(int eventId);
    Task<List<Participant>> GetParticipationsByUserAsync(string userId);

    Task<Participant> RegisterParticipantAsync(Participant participant);
    Task<bool> UnregisterParticipantAsync(int participantId);
    Task<bool> CheckInParticipantAsync(int participantId);

    Task<bool> RegisterUserToEventAsync(int eventId, string userId);
    Task<bool> IsUserRegisteredAsync(int eventId, string userId);
    Task<bool> CancelRegistrationAsync(int eventId, string userId);
}