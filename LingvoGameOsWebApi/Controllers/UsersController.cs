using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet("{userId}/points")]
        public async Task<ActionResult<int>> GetUserPoints(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Пользователя с таким id не существует");
            int points = user.TotalPoints;
            return Ok(points);
        }

        [HttpPost("{userId}/points")]
        public async Task<ActionResult> AddPoints(string userId, [FromBody] AddPointsRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Пользователя с таким id не существует");

            if (request.Amount <= 0)
                return BadRequest("Количество баллов должно быть положительным");

            user.TotalPoints += request.Amount;
            int newTotalPoints = user.TotalPoints;
            await _userManager.UpdateAsync(user);
            return Ok(new { UserId = userId, NewTotalPoints = newTotalPoints });
        }
    }

    public record AddPointsRequest(int Amount);
}
