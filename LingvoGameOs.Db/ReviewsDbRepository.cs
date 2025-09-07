using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class ReviewsDbRepository : IReviewsRepository
    {
        readonly DatabaseContext _databaseContext;
        readonly IMemoryCache _memoryCache;
        const string CacheReviewKey = "reviews_cache_key";

        public ReviewsDbRepository(DatabaseContext databaseContext, IMemoryCache memoryCache)
        {
            _databaseContext = databaseContext;
            _memoryCache = memoryCache;
        }

        public async Task<Review?> TryGetByIdAsync(Guid reviewId)
        {
            return await _databaseContext.Reviews
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task AddAsync(Review review)
        {
            await _databaseContext.Reviews.AddAsync(review);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<Review>> GetAllAsync()
        {
            if (!_memoryCache.TryGetValue(CacheReviewKey, out List<Review>? reviews))
            {
                reviews = await _databaseContext.Reviews
                    .Include(r => r.Author)
                    .Include(r => r.Game)
                    .OrderByDescending(r => r.PublicationDate)
                    .AsSplitQuery()
                    .ToListAsync();
                _memoryCache.Set(CacheReviewKey, reviews, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
            }

            return reviews!;
        }

        public Task InvalidateCacheAsync()
        {
            _memoryCache.Remove(CacheReviewKey);
            return Task.CompletedTask;
        }
    }
}
