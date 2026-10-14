using LingvoGameOs.Db;
using LingvoGameOsWebApi.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LingvoGameOsWebApi.Controllers;

[ApiController]
[Route("api/developer")]
[Authorize(Policy = DeveloperApiKeyAuthenticationDefaults.Policy)]
public sealed class DeveloperController : ControllerBase
{
    readonly DatabaseContext _dbContext;

    public DeveloperController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            status = "ok",
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            apiKeyId = User.FindFirstValue(DeveloperApiKeyAuthenticationDefaults.ApiKeyIdClaim),
        });
    }

    [HttpGet("games")]
    [Authorize(Policy = DeveloperApiKeyAuthenticationDefaults.GamesReadPolicy)]
    public async Task<IActionResult> GetGamesAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var games = await _dbContext.Games
            .AsNoTracking()
            .Where(game => game.AuthorId == userId)
            .OrderByDescending(game => game.PublicationDate)
            .Select(game => new
            {
                id = game.Id,
                title = game.Title,
                publicationDate = game.PublicationDate,
                isActive = game.IsActive,
            })
            .ToListAsync(cancellationToken);

        return Ok(games);
    }
}
