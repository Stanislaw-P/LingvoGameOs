using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Helpers
{
    public class RatingService
    {
        readonly DatabaseContext _databaseContext;

        public RatingService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task UpdateGameRating(int gameId, int newRating)
        {
            var existingGame = await _databaseContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);
            if (existingGame == null)
                return;

            existingGame.TotalReviews += 1;
            existingGame.TotalRatingPoints += newRating;
            existingGame.AverageRaitingPlayers = Math.Round((double)existingGame.TotalRatingPoints / existingGame.TotalReviews, 1);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task CalculateRatingAfterDelete(int gameId, int deletedRating)
        {
            var existingGame = await _databaseContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);
            if (existingGame == null)
                return;

            existingGame.TotalRatingPoints -= deletedRating;
            existingGame.TotalReviews -= 1;
            existingGame.AverageRaitingPlayers = Math.Round((double)existingGame.TotalRatingPoints / existingGame.TotalReviews, 1);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
