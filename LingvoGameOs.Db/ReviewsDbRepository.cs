using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
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

        public ReviewsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
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
            return await _databaseContext.Reviews
                .Include(r => r.Author)
                .Include(r => r.Game)
                .OrderByDescending(r => r.PublicationDate)
                .AsSplitQuery()
                .ToListAsync();
        }
    }
}
