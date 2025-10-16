
using LingvoGameOs.Db;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;

namespace LingvoGameOs.Helpers
{
    public class ReviewCache : BackgroundService
    {
        readonly IMemoryCache _memoryCache;
        readonly IServiceProvider _serviceProvider;

        public ReviewCache(IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CachingReviewsAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CachingReviewsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReviewsRepository>();

                var reviews = await repository.GetAllAsync();

                if (reviews == null)
                    return;
                
                _memoryCache.Set(Constants.CacheAllReviewsKey, reviews, TimeSpan.FromMinutes(10));
            }
        }
    }
}
