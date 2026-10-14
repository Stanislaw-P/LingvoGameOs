using LingvoGameOs.Db;
using LingvoGameOs.Shared.Services;
using LingvoGameOsWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : Controller
    {
        readonly IGameSessionRepository _sessionRepository;
        readonly IConfiguration _configuration;

        public GameController(
            IGameSessionRepository sessionRepository,
            IConfiguration configuration)
        {
            _sessionRepository = sessionRepository;
            _configuration = configuration;
        }

        [HttpPost("session/resolve")]
        public async Task<IActionResult> ResolveGameSessionAsync([FromBody] GameSessionRequest request)
        {
            if (request.gameSessionId <= 0)
                return BadRequest("Некорректный идентификатор игровой сессии");

            var apiSecret = _configuration["GAMES_API_SECRET"]
                ?? throw new InvalidOperationException("GAMES_API_SECRET is not configured");

            // Не раскрываем пользователя по одному только последовательному id:
            // данные сессии доступны лишь предъявителю выданной порталом подписи.
            if (!GameSessionHmac.Verify(request.gameSessionId, request.token ?? string.Empty, apiSecret))
                return Unauthorized("Неверная подпись игровой сессии");

            var existingSession = await _sessionRepository.TryGetByIdAsync(request.gameSessionId);
            if (existingSession == null)
            {
                return NotFound($"Сессии с id:'{request.gameSessionId}' не существует");
            }

            var response = new GameSessionResponse(true, existingSession.GameId, existingSession.UserId, existingSession.User.Name, existingSession.User.TotalPoints);

            return Ok(response);
        }
    }
}
