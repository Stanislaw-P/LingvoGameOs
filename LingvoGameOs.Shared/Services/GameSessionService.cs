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
            try
            {
                var session = new GameSession
                {
                    UserId = userId,
                    GameId = gameId,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _sessionRepository.AddAsync(session);

                return session;
            }
            catch (Exception ex)
            {
                return new GameSession();
            }
        }
    }
}
