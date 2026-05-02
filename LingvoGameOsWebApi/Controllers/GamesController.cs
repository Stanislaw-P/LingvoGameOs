using LingvoGameOs.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LingvoGameOsWebApi.Controllers
{
    [Route("api/games")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        readonly IGamesRepository _gamesRepository;

        public GamesController(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        [HttpGet("my")]
        public async Task<ActionResult> GetDevGames()
        {
            try
            {
                string? devUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (devUserId == null)
                    return Unauthorized();

                var devGames = await _gamesRepository.TryGetUserDevGamesAsync(devUserId);
                
                // Добавляем всем разрабам в список тестовую игру, если это яндекс
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
                {
                    var testGame = await _gamesRepository.TryGetByIdAsync(8);
                    devGames.Add(testGame!);
                }

                var response = devGames.Select(g => new
                {
                    gameTitle = g.Title,
                    gameId = g.Id,
                    gamePublicationDate = g.PublicationDate
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
