using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface IReviewsRepository
    {
        Task<Review?> TryGetUserReviewAsync(string userId, int gameId);
        Task<Review?> TryGetByIdAsync(Guid reviewId);
        Task AddAsync(Review review);
        Task<List<Review>> GetAllAsync();
        Task PublishAsync(Guid reviewId);
        Task DeleteAsync(Guid reviewId);
        Task UpdateAsync(Review review);
    }
}