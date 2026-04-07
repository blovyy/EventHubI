using EventHub.Data.Models;

namespace EventHub.Services.Interfaces;

public interface IReviewService
{
    Task<bool> CreateReviewAsync(Review review);
    Task<List<Review>> GetReviewsByUserAsync(string userId);
    Task<List<Review>> GetApprovedReviewsAsync();
    Task<List<Review>> GetApprovedReviewsByEventAsync(int eventId);
    Task<List<Review>> GetPendingReviewsAsync();
    Task<bool> ApproveReviewAsync(int reviewId);
    Task<bool> RejectReviewAsync(int reviewId);
    Task<bool> CanUserLeaveReviewAsync(int eventId, string userId);
}