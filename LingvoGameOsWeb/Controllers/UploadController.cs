using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LingvoGameOs.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly UserManager<User> _userManager;
        readonly FileProvider _fileProvider;

        public UploadController(IGamesRepository gamesRepository, UserManager<User> userManager, IWebHostEnvironment appEnvironment)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _fileProvider = new FileProvider(appEnvironment);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(AddGameViewModel gameViewModel)
        {
            if (gameViewModel.UploadedGame == null && gameViewModel.GameURL == null)
            {
                ModelState.AddModelError("", "Необходимо добавить ссылку на игру, либо загрузить файл игры");
                return View(gameViewModel);
            }

            if (gameViewModel.UploadedGame != null && gameViewModel.GameURL != null)
            {
                ModelState.AddModelError("", "Можно выбрать только одну платформу для игры");
                return View(gameViewModel);
            }

            if (ModelState.IsValid)
            {
                var imagesPaths = await _fileProvider.SafeImagesFilesAsync(gameViewModel.UploadedImages, ImageFolders.Games);
                
                if (gameViewModel.UploadedGame != null)
                {
                    Game newGame = new Game
                    {
                        Title = gameViewModel.Title,
                        Description = gameViewModel.Description,
                        Rules = gameViewModel.Rules,
                        AuthorId = _userManager.GetUserId(User),
                        ImagesURLs = imagesPaths,
                        PublicationDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,

                    };
                }
                else
                {

                }
            }

            return View(gameViewModel);
        }
    }
}
