using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly UserManager<User> _userManager;
        readonly FileProvider _fileProvider;
        readonly DatabaseContext _databaseContext;
        readonly Utilities _utilities;

        public UploadController(IGamesRepository gamesRepository, UserManager<User> userManager, IWebHostEnvironment appEnvironment, DatabaseContext databaseContext)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _fileProvider = new FileProvider(appEnvironment);
            _databaseContext = databaseContext;
            _utilities = new Utilities(databaseContext);
        }

        public IActionResult Index()
        {
            ViewBag.SkillsLearning = _databaseContext.SkillsLearning.Select(type => type.Name);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] AddGameViewModel gameViewModel)
        {
            //var items = JsonSerializer.Deserialize<List<string>>(gameViewModel.SkillsLearning);
            //var skills = await _utilities.GetSkillsAsync(items);
            //var platform = await _utilities.GetGamePlatformAsync(gameViewModel.GamePlatform);
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> IndexAsync(AddGameViewModel gameViewModel)
        //{
        //    if (gameViewModel.UploadedGame == null && gameViewModel.GameURL == null)
        //    {
        //        ModelState.AddModelError("", "Необходимо добавить ссылку на игру, либо загрузить файл игры");
        //        return View(gameViewModel);
        //    }

        //    if (gameViewModel.UploadedGame != null && gameViewModel.GameURL != null)
        //    {
        //        ModelState.AddModelError("", "Можно выбрать только одну платформу для игры");
        //        return View(gameViewModel);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var imagesPaths = await _fileProvider.SafeImagesFilesAsync(gameViewModel.UploadedImages, ImageFolders.Games);
                
        //        if (gameViewModel.UploadedGame != null)
        //        {
        //            Game newGame = new Game
        //            {
        //                Title = gameViewModel.Title,
        //                Description = gameViewModel.Description,
        //                Rules = gameViewModel.Rules,
        //                AuthorId = _userManager.GetUserId(User),
        //                ImagesURLs = imagesPaths,
        //                PublicationDate = DateTime.Now,
        //                LastUpdateDate = DateTime.Now,
        //                // TODO: Нужно придумать как получить из формы выбранные скилы для игры
        //            };
        //        }
        //        else
        //        {
        //            // TODO: Если игра не десктоп, то она веб...
        //        }
        //    }

        //    return View(gameViewModel);
        //}
    }
}
