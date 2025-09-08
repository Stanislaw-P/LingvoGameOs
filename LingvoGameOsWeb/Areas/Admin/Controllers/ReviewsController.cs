using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
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
        public ReviewsController(IReviewsRepository reviewsRepository, UserManager<User> userManager)
        {
            _reviewsRepository = reviewsRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
                return NotFound();

            var reviews = await _reviewsRepository.GetAllAsync();
            int approvedReviewsCount =reviews.Count(r => r.IsApproved == true);
            
            ViewBag.ApprovedReviewsCount = approvedReviewsCount;
            ViewBag.NotApprovedReviewsCount = reviews.Count - approvedReviewsCount;
            ViewBag.UsersCount = _userManager.Users.Count();
            ViewBag.AdminName = $"{adminUser.Name} {adminUser.Surname}";

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> PublishAsync([FromBody] Guid reviewId)
        {
            try
            {
                await _reviewsRepository.PublishAsync(reviewId);
                await _reviewsRepository.InvalidateCacheAsync();
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
                await _reviewsRepository.DeleteAsync(reviewId);
                await _reviewsRepository.InvalidateCacheAsync();
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
