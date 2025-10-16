using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;


namespace LingvoGameOs.Db
{
    public class ReviewsDbRepository : IReviewsRepository
    {
        readonly DatabaseContext _databaseContext;

        public ReviewsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Review?> TryGetUserReviewAsync(string userId, int gameId)
        {
            return await _databaseContext.Reviews
                .FirstOrDefaultAsync(r => r.Author.Id == userId && r.GameId == gameId);
        }

        public async Task AddAsync(Review review)
        {
            await _databaseContext.Reviews.AddAsync(review);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await _databaseContext.Reviews
                 .Include(r => r.Author)
                 .Include(r => r.Game)
                 .OrderByDescending(r => r.PublicationDate)
                 .AsSplitQuery()
                 .ToListAsync();
        }

        public async Task PublishAsync(Guid reviewId)
        {
            var existingReview = await _databaseContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (existingReview != null)
            {
                existingReview.IsApproved = true;
                await _databaseContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Review review)
        {
            var existingReview = _databaseContext.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (existingReview == null)
                throw new InvalidOperationException($"Отзыв с Id {review.Id} не найдена :(");
            _databaseContext.Reviews.Update(review);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid reviewId)
        {
            var existingReview = await TryGetByIdAsync(reviewId);
            if (existingReview != null)
            {
                _databaseContext.Reviews.Remove(existingReview);
                await _databaseContext.SaveChangesAsync();
            }
        }

        private async Task<Review?> TryGetByIdAsync(Guid reviewId)
        {
            return await _databaseContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        }
    }
}
