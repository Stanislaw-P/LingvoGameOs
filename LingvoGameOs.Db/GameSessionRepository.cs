using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class GameSessionRepository : IGameSessionRepository
    {
        readonly DatabaseContext _context;

        public GameSessionRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddAsync(GameSession session)
        {
            await _context.GameSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<GameSession?> TryGetAsync(string userId, int gameId)
        {
            return await _context.GameSessions.FirstOrDefaultAsync(gs => gs.UserId == userId && gs.GameId == gameId);
        }

        public async Task<GameSession?> TryGetByIdAsync(int id)
        {
            return await _context.GameSessions.FirstOrDefaultAsync(gs => gs.Id == id);
        }
    }
}
