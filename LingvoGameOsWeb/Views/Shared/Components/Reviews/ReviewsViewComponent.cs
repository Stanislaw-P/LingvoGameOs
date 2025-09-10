using LingvoGameOs.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Views.Shared.Components.Reviews
{
    public class ReviewsViewComponent : ViewComponent
    {
        readonly IReviewsRepository _reviewsRepository;

        public ReviewsViewComponent(IReviewsRepository reviewsRepository)
        {
            _reviewsRepository = reviewsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool isDetailsPage, int gameId)
        {
            var allReviews = await _reviewsRepository.GetAllAsync();
            ViewBag.IsDetailsPage = isDetailsPage;
            ViewBag.GameId = gameId;
            return View("Reviews", allReviews);
        }
    }
}
