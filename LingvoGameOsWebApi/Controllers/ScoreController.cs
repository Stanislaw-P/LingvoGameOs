using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOsWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/score")]
    public class ScoreController : Controller
    {
        readonly IGameSessionRepository _sessionRepository;
        readonly UserManager<User> _userManager;

        public ScoreController(IGameSessionRepository sessionRepository, UserManager<User> userManager)
        {
            _sessionRepository = sessionRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddScoreAsync([FromBody] GameScoreRequest request)
        {
            var existingSession = await _sessionRepository.TryGetByIdAsync(request.gameSessionId);
            if (existingSession == null)
            {
                return NotFound($"Сессии с id:'{request.gameSessionId}' не существует");
            }

            if (request.score <= 0)
                return BadRequest("Количество баллов должно быть положительным");

            existingSession.User.TotalPoints += request.score;
            await _userManager.UpdateAsync(existingSession.User);

            return Ok(new { UserId = existingSession.UserId, NewTotalPoints = existingSession.User.TotalPoints });

        }
    }
}
