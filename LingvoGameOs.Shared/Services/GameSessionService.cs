using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.Extensions.Logging;

namespace LingvoGameOs.Shared.Services
{
    public class GameSessionService
    {
        readonly IGameSessionRepository _sessionRepository;

        public GameSessionService(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<GameSession> CreateGameSessionAsync(string userId, int gameId)
        {
            var session = new GameSession
            {
                UserId = userId,
                GameId = gameId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Ошибка создания сессии должна прервать запуск игры. Объект с Id=0
            // нельзя подписывать и выдавать как успешно созданную сессию.
            await _sessionRepository.AddAsync(session);

            return session;
        }
    }
}
