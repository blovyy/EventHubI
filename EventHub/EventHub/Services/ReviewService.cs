using EventHub.Data;
using EventHub.Data.Models;
using EventHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Services;

public class ReviewService : IReviewService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ReviewService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<bool> CreateReviewAsync(Review review)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var alreadyExists = await context.Reviews
            .AnyAsync(x => x.EventId == review.EventId && x.UserId == review.UserId);

        if (alreadyExists)
            return false;

        var canLeave = await CanUserLeaveReviewAsync(review.EventId, review.UserId);
        if (!canLeave)
            return false;

        review.CreatedAt = DateTime.UtcNow;
        review.ModerationStatus = ReviewModerationStatus.Pending;

        context.Reviews.Add(review);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Review>> GetReviewsByUserAsync(string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Reviews
            .Include(x => x.Event)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Review>> GetApprovedReviewsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Reviews
            .Include(x => x.Event)
            .Include(x => x.User)
            .Where(x => x.ModerationStatus == ReviewModerationStatus.Approved)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Review>> GetApprovedReviewsByEventAsync(int eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Reviews
            .Include(x => x.User)
            .Include(x => x.Event)
            .Where(x => x.EventId == eventId && x.ModerationStatus == ReviewModerationStatus.Approved)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Review>> GetPendingReviewsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Reviews
            .Include(x => x.Event)
            .Include(x => x.User)
            .Where(x => x.ModerationStatus == ReviewModerationStatus.Pending)
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ApproveReviewAsync(int reviewId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
        if (review == null)
            return false;

        review.ModerationStatus = ReviewModerationStatus.Approved;
        review.ModeratedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectReviewAsync(int reviewId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
        if (review == null)
            return false;

        review.ModerationStatus = ReviewModerationStatus.Rejected;
        review.ModeratedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanUserLeaveReviewAsync(int eventId, string userId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var ev = await context.Events
            .FirstOrDefaultAsync(x => x.Id == eventId);

        if (ev == null)
            return false;

        // Review is allowed only after the event has fully ended.
        if (ev.EndDate > DateTime.Now)
            return false;

        var participated = await context.Participants
            .AnyAsync(x => x.EventId == eventId && x.UserId == userId);

        if (!participated)
            return false;

        var reviewExists = await context.Reviews
            .AnyAsync(x => x.EventId == eventId && x.UserId == userId);

        return !reviewExists;
    }
}
