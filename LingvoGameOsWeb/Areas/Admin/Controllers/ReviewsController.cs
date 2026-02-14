using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LingvoGameOs.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class ReviewsController : Controller
    {
        readonly IReviewsRepository _reviewsRepository;
        readonly UserManager<User> _userManager;
        readonly RatingService _ratingService;
        readonly S3Service _s3Service;

        public ReviewsController(IReviewsRepository reviewsRepository, UserManager<User> userManager, RatingService ratingService, S3Service s3Service)
        {
            _reviewsRepository = reviewsRepository;
            _userManager = userManager;
            _ratingService = ratingService;
            _s3Service = s3Service;
        }

        public async Task<IActionResult> Index()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
                return NotFound();

            var reviews = await _reviewsRepository.GetAllAsync();
            int approvedReviewsCount = reviews.Count(r => r.IsApproved == true);

            ViewBag.ApprovedReviewsCount = approvedReviewsCount;
            ViewBag.NotApprovedReviewsCount = reviews.Count - approvedReviewsCount;
            ViewBag.UsersCount = _userManager.Users.Count();
            ViewBag.AdminName = $"{adminUser.Name} {adminUser.Surname}";

            var reviewsVm = reviews?.Select(r => new ReviewViewModel
            {
                Review = r,
                AuthorAvatarUrl = _s3Service.GetPublicUrl(r.Author?.AvatarImgPath!)
            }).ToList();

            return View(reviewsVm);
        }

        [HttpPost]
        public async Task<IActionResult> PublishAsync([FromBody] Guid reviewId)
        {
            try
            {
                var existingReview = await _reviewsRepository.TryGetByIdAsync(reviewId);
                if (existingReview == null)
                    return BadRequest(new { success = false, message = "Отзыв с таким id не найден!" });

                await _reviewsRepository.PublishAsync(reviewId);

                await _ratingService.UpdateGameRatingAsync(existingReview.GameId, existingReview.Rating);

                return Ok(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync([FromBody] Guid reviewId)
        {
            try
            {
                var existingReview = await _reviewsRepository.TryGetByIdAsync(reviewId);
                if (existingReview == null)
                    return BadRequest(new { success = false, message = "Отзыв с таким id не найден!" });

                await _ratingService.CalculateRatingAfterDeleteAsync(existingReview.GameId, existingReview.Rating);
                await _reviewsRepository.DeleteAsync(reviewId);
                return Ok(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
