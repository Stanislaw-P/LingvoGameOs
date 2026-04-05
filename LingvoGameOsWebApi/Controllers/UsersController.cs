using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LingvoGameOsWebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        readonly UserManager<User> _userManager;
        readonly ILogger<UsersController> _logger;
        readonly IGamesRepository _gamesRepository;
        readonly IHttpClientFactory _httpClientFactory;

        public UsersController(UserManager<User> userManager, ILogger<UsersController> logger, IGamesRepository gamesRepository, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _logger = logger;
            _gamesRepository = gamesRepository;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet("{userId}/points")]
        public async Task<ActionResult<int>> GetUserPointsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"User with ID '{userId}' not found.");
            int points = user.TotalPoints;
            return Ok(new
            {
                points
            });
        }

        [HttpPost("{userId}/games/{gameId}/points")]
        public async Task<ActionResult> AddPointsAsync(string userId, int gameId, [FromBody] AddPointsRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest("UserId is required.");

                if (gameId <= 0)
                    return BadRequest("GameId must be greater than 0.");

                if (request == null)
                    return BadRequest("Request body is required.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound($"User with ID '{userId}' not found.");

                if (request.Amount <= 0)
                    return BadRequest("Amount must be greater than 0.");

                var game = await _gamesRepository.TryGetByIdAsync(gameId);
                if (game == null)
                    return NotFound("There is no game with this ID.");

                if (game.AuthorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                    return BadRequest($"You are not the author of the game with ID: {gameId}");

                user.TotalPoints += request.Amount;
                int newTotalPoints = user.TotalPoints;
                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    UserId = userId,
                    GameId = gameId,
                    PointsAdded = request.Amount,
                    NewTotalPoints = newTotalPoints
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding points for UserId: {UserId}, GameId: {GameId}", userId, gameId);
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        //[HttpGet("current-user-id")]
        //public async Task<ActionResult> GetCurrentUserIdAsync()
        //{
        //    try
        //    {
        //        var client = _httpClientFactory.CreateClient("MvcClient");

        //        client.BaseAddress = new Uri("http://localhost:5086");

        //        // Вызываем эндпоинт MVC
        //        var response = await client.GetAsync("/api/mvc/current-user-id");

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            _logger.LogError("Failed to get user ID from MVC. Status: {StatusCode}", response.StatusCode);
        //            return BadRequest(response.StatusCode);
        //        }

        //        var result = await response.Content.ReadFromJsonAsync<UserIdResponse>();
        //        return Ok(new { userId = result?.UserId });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting user ID from MVC");
        //        return BadRequest(ex);
        //    }
        //}
    }

    public record AddPointsRequest(int Amount);
    record UserIdResponse(string UserId);
}
