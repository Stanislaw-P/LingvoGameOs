using LingvoGameOs.Db;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LingvoGameOs.Views.Shared.Components.FooterStatistics
{
    public class FooterStatisticsViewComponent : ViewComponent
    {
        readonly DatabaseContext _databaseContext;
        readonly IMemoryCache _memoryCache;
        const string STATISTICS_CACHE_KEY = "FooterStatistics";
        static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30); // Кеш на 30 минут

        public FooterStatisticsViewComponent(IMemoryCache memoryCache, DatabaseContext databaseContext)
        {
            _memoryCache = memoryCache;
            _databaseContext = databaseContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var statistics = await _memoryCache.GetOrCreateAsync(STATISTICS_CACHE_KEY, async entry =>
            {
                // Устанавливаем время жизни кеша
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                // Добавляем приоритет - не удалять при нехватке памяти
                entry.Priority = CacheItemPriority.High;

                // Получаем данные из БД только при отсутствии в кеше
                return new FooterStatisticsViewModel
                {
                    GamesCount = await _databaseContext.Games.CountAsync(),
                    UsersCount = await _databaseContext.Users.CountAsync(),
                };
            });

            return View("Statistics", statistics);
        }
    }
}
