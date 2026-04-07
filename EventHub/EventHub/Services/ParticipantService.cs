using EventHub.Data;
using EventHub.Data.Models;
using EventHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Services;

public class ParticipantService : IParticipantService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ParticipantService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Participant>> GetAllParticipantsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Participants
            .Include(p => p.Event)
            .Include(p => p.User)
            .OrderByDescending(p => p.RegisteredAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Participant?> GetParticipantByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Participants
            .Include(p => p.Event)
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Participant>> GetParticipantsByEventAsync(int eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Participants
            .Include(p => p.User)
            .Where(p => p.EventId == eventId)
            .OrderByDescending(p => p.RegisteredAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Participant>> GetParticipationsByUserAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Participants
            .Include(p => p.Event)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.RegisteredAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Participant> RegisterParticipantAsync(Participant participant)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        participant.RegisteredAt = DateTime.UtcNow;
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        return participant;
    }

    public async Task<bool> UnregisterParticipantAsync(int participantId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var participant = await context.Participants.FirstOrDefaultAsync(p => p.Id == participantId);
        if (participant == null)
            return false;

        context.Participants.Remove(participant);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckInParticipantAsync(int participantId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var participant = await context.Participants.FirstOrDefaultAsync(p => p.Id == participantId);
        if (participant == null)
            return false;

        if (!participant.IsCheckedIn)
        {
            participant.IsCheckedIn = true;
            participant.CheckedInAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> IsUserRegisteredAsync(int eventId, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Participants
            .AnyAsync(p => p.EventId == eventId && p.UserId == userId);
    }

    public async Task<bool> RegisterUserToEventAsync(int eventId, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ev = await context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null)
            return false;

        if (ev.ModerationStatus != EventModerationStatus.Approved || ev.Status != EventStatus.Published)
            return false;

        if (ev.StartDate <= DateTime.Now)
            return false;

        var alreadyRegistered = await context.Participants
            .AnyAsync(p => p.EventId == eventId && p.UserId == userId);

        if (alreadyRegistered)
            return false;

        var participantsCount = await context.Participants
            .CountAsync(p => p.EventId == eventId);

        if (participantsCount >= ev.MaxParticipants)
            return false;

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return false;

        var fullName = string.Join(" ", new[] { user.FirstName, user.LastName }
            .Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

        var participant = new Participant
        {
            EventId = eventId,
            UserId = userId,
            FullName = string.IsNullOrWhiteSpace(fullName) ? user.UserName : fullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RegisteredAt = DateTime.UtcNow
        };

        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelRegistrationAsync(int eventId, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var participant = await context.Participants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

        if (participant == null)
            return false;

        context.Participants.Remove(participant);
        await context.SaveChangesAsync();

        return true;
    }
}