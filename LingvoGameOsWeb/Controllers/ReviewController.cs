using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    public class ReviewController : Controller
    {
        readonly UserManager<User> _userManager;
        readonly ILogger<ReviewController> _logger;
        readonly IReviewsRepository _reviewsRepository;
        readonly IGamesRepository _gamesRepository;

        public ReviewController(UserManager<User> userManager, ILogger<ReviewController> logger, IReviewsRepository reviewsRepository, IGamesRepository gamesRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _reviewsRepository = reviewsRepository;
            _gamesRepository = gamesRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SendAsync([FromBody] Review review)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return BadRequest(new { success = false, message = "Войдите в аккаунт, чтобы написать отзыв!" });

            var existingGame = await _gamesRepository.TryGetByIdAsync(review.GameId);
            if (existingGame == null)
                return BadRequest(new { success = false, message = $"Игра с id:{review.GameId} не существует!" });

            var logData = new
            {
                PendingGameId = existingGame.Id,
                UserId = currentUser.Id,
                UserIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString(),
            };

            try
            {
                review.Author = currentUser;
                review.PublicationDate = DateTime.Now;
                await _reviewsRepository.AddAsync(review);
                // Очищаем Кеш после добавления отзыва
                await _reviewsRepository.InvalidateCacheAsync();
                _logger.LogInformation("Успешная отправка отзыва {@SendReviewsData}", new
                {
                    logData.UserId,
                    logData.UserIP,
                    logData.UserAgent,
                    RequestTime = DateTime.UtcNow,
                    GameId = existingGame.Id,
                    ReviewId = review.Id,
                    ResponseStatusCode = 200
                });

                return Ok(new
                {
                    success = true,
                    reviewData = new
                    {
                        text = review.Text,
                        rating = review.Rating,
                        publicationDate = review.PublicationDate,
                        authorAvatarPath = currentUser.AvatarImgPath,
                        authorName = $"{currentUser.Name} {currentUser.Surname}",
                        gameTitle = existingGame.Title
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки отзыва {@SendReviewsData}", new
                {
                    logData.UserId,
                    logData.UserIP,
                    logData.UserAgent,
                    RequestTime = DateTime.UtcNow,
                    GameId = existingGame.Id,
                    ResponseStatusCode = 500
                });
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
