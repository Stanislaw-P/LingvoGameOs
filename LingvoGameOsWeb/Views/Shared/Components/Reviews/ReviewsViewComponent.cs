using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace LingvoGameOs.Views.Shared.Components.Reviews
{
    public class ReviewsViewComponent : ViewComponent
    {
        readonly IReviewsRepository _reviewsRepository;
        readonly IMemoryCache _memoryCache;

        public ReviewsViewComponent(IReviewsRepository reviewsRepository, IMemoryCache memoryCache)
        {
            _reviewsRepository = reviewsRepository;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool isDetailsPage, int gameId)
        {
            if (!_memoryCache.TryGetValue<List<Review>>(Constants.CacheAllReviewsKey, out var allReviews))
            {
                allReviews = await _reviewsRepository.GetAllAsync();
                _memoryCache.Set(Constants.CacheAllReviewsKey, allReviews, TimeSpan.FromMinutes(10));
            }
            ViewBag.IsDetailsPage = isDetailsPage;
            ViewBag.GameId = gameId;
            return View("Reviews", allReviews);
        }
    }
}
