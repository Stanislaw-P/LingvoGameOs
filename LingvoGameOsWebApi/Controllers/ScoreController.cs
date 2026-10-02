using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Shared.Services;
using LingvoGameOsWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/score")]
    public class ScoreController : Controller
    {
        // Максимум баллов за одно начисление (защита от инфляции и overflow)
        const int MaxScorePerRequest = 1000;

        readonly IGameSessionRepository _sessionRepository;
        readonly UserManager<User> _userManager;
        readonly IConfiguration _configuration;

        public ScoreController(
            IGameSessionRepository sessionRepository,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _sessionRepository = sessionRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [EnableRateLimiting("score")]
        public async Task<IActionResult> AddScoreAsync([FromBody] GameScoreRequest request)
        {
            if (request.score <= 0 || request.score > MaxScorePerRequest)
                return BadRequest($"Количество баллов должно быть от 1 до {MaxScorePerRequest}");

            // HMAC-подпись игровой сессии: без неё (или с неверной) начисление отклоняется.
            // Подпись выдаёт портал при запуске игры и передаёт в iframe вместе с session id.
            var apiSecret = _configuration["GAMES_API_SECRET"]
                ?? throw new InvalidOperationException("GAMES_API_SECRET is not configured");
            if (!GameSessionHmac.Verify(request.gameSessionId, request.token, apiSecret))
                return Unauthorized("Неверная подпись игровой сессии");

            var existingSession = await _sessionRepository.TryGetByIdAsync(request.gameSessionId);
            if (existingSession == null)
            {
                return NotFound($"Сессии с id:'{request.gameSessionId}' не существует");
            }

            if (existingSession.Used)
            {
                // сессия одноразовая: повторное начисление по той же сессии запрещено
                return Conflict($"Сессия с id:'{request.gameSessionId}' уже использована");
            }

            // Полностью атомарно: захват сессии (Used=false→true) и инкремент одним statement'ом
            var updated = await _sessionRepository.AddScoreAsync(request.gameSessionId, request.score);
            if (updated == null)
            {
                return Conflict($"Сессия с id:'{request.gameSessionId}' уже использована");
            }


            return Ok(new { UserId = existingSession.UserId, NewTotalPoints = updated.Value });
        }
    }
}
