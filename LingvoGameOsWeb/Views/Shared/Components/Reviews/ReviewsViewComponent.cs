using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
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
        readonly IFileStorage _fileStorage;

        public ReviewsViewComponent(IReviewsRepository reviewsRepository, IMemoryCache memoryCache, IFileStorage fileStorage)
        {
            _reviewsRepository = reviewsRepository;
            _memoryCache = memoryCache;
            _fileStorage = fileStorage;
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

            var reviewsVm = allReviews?.Select(r => new ReviewViewModel
            {
                Review = r,
                AuthorAvatarUrl = _fileStorage.GetPublicUrl(r.Author?.AvatarImgPath!)
            }).ToList();
            return View("Reviews", reviewsVm);
        }
    }
}
