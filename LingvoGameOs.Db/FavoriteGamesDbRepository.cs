﻿using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class FavoriteGamesDbRepository : IFavoriteGamesRepository
    {
        readonly DatabaseContext _databaseContext;
        readonly UserManager<User> _userManager;
        readonly IGamesRepository _gamesRepository;

        public FavoriteGamesDbRepository(DatabaseContext databaseContext, UserManager<User> userManager, IGamesRepository gamesRepository)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
            _gamesRepository = gamesRepository;
        }

        public async Task<bool> AddAsync(string userId, int gameId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var game = await _gamesRepository.TryGetByIdAsync(gameId);
            if (game == null) return false;

            var existingFavorite = await TryGetFavoriteAsync(userId, gameId);
            if (existingFavorite != null) return false;

            var favorite = new FavoriteGame
            {
                GameId = gameId,
                UserId = userId,
                DateAdded = DateTimeOffset.UtcNow,
            };

            await _databaseContext.FavoriteGames.AddAsync(favorite);
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int gameId)
        {
            var existingFavorite = await TryGetFavoriteAsync(userId, gameId);
            if (existingFavorite == null) return false;

            _databaseContext.FavoriteGames.Remove(existingFavorite);
            await _databaseContext.SaveChangesAsync();

            return true;
        }

        public async Task<FavoriteGame?> TryGetFavoriteAsync(string userId, int gameId)
        {
            return await _databaseContext.FavoriteGames
                .FirstOrDefaultAsync(f => f.UserId == userId && f.GameId == gameId);
        }

        public async Task<List<Game>> GetUserFavoritesAsync(string userId)
        {
            return await _databaseContext.FavoriteGames
                .Where(f => f.UserId == userId)
                .Include(f => f.Game)
                .Include(f => f.Game.GamePlatform)
                .AsSplitQuery()
                .Select(f => f.Game)
                .ToListAsync();
        }

        public async Task<int> GetGameFavoritesCountAsync(int gameId)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(gameId);
            if(existingGame == null) 
                return 0;
            return existingGame?.FavoriteGames?.Count ?? 0;
        }

        public async Task<bool> IsGameInFavoritesAsync(string userId, int gameId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            var existingFavorite = await TryGetFavoriteAsync(userId, gameId);
            return existingFavorite != null;

        }
    }
}
