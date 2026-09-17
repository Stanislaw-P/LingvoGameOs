using LingvoGameOs.Db;
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

        public GameController(IGameSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetGameSessionAsync(int sessionId)
        {
            var existingSession = await _sessionRepository.TryGetByIdAsync(sessionId);
            if (existingSession == null)
            {
                return NotFound($"Сессии с id:'{sessionId}' не существует");
            }

            var response = new GameSessionResponse(true, existingSession.GameId, existingSession.UserId, existingSession.User.Name, existingSession.User.TotalPoints);

            return Ok(response);
        }
    }
}
