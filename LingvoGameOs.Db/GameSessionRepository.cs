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
            return await _context.GameSessions
                .Include(gs => gs.User)
                .FirstOrDefaultAsync(gs => gs.Id == id);
        }

        // Полностью атомарное начисление одним statement'ом:
        // 1) помечает сессию использованной (WHERE Used = false),
        // 2) прибавляет баллы пользователю сессии.
        // Возвращает новый итог или null, если сессии нет / уже использована.
        // Гонок нет: БД сериализует и Used, и инкремент в одной транзакции.
        public async Task<int?> AddScoreAsync(int gameSessionId, int score)
        {
            // Шаг 1 (атомарный захват): UPDATE ... WHERE Used=false возвращает UserId.
            // Конкурентный запрос увидит Used=true и получит 0 строк.
            var claimed = await _context.Database
                .SqlQueryRaw<string>(
                    @"UPDATE ""GameSessions""
                      SET ""Used"" = true
                      WHERE ""Id"" = @sessionId AND ""Used"" = false
                      RETURNING ""UserId""::text;",
                    new Npgsql.NpgsqlParameter("sessionId", gameSessionId))
                .ToListAsync();

            if (claimed.Count == 0)
                return null;


            // Шаг 2: инкремент одним statement'ом — параллельные инкременты не теряются.
            var rows = await _context.Database
                .SqlQueryRaw<int>(
                    @"UPDATE ""AspNetUsers""
                      SET ""TotalPoints"" = ""TotalPoints"" + @score
                      WHERE ""Id"" = @userId
                      RETURNING ""TotalPoints"";",
                    new Npgsql.NpgsqlParameter("userId", claimed[0]),
                    new Npgsql.NpgsqlParameter("score", score))
                .ToListAsync();

            return rows.Count > 0 ? rows[0] : (int?)null;
        }

        // Помечает сессию использованной (одноразовость)
        public async Task MarkUsedAsync(int gameSessionId)
        {
            await _context.Database
                .ExecuteSqlInterpolatedAsync(
                    $"UPDATE \"GameSessions\" SET \"Used\" = true WHERE \"Id\" = {gameSessionId}");
        }
    }
}
