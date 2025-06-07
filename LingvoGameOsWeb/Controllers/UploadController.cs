using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LingvoGameOs.Models;
using Microsoft.EntityFrameworkCore;

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

        //[HttpPost]
        //public async Task<IActionResult> Index([FromForm] AddGameViewModel gameViewModel)
        //{
        //    var items = JsonSerializer.Deserialize<List<string>>(gameViewModel.SkillsLearning);
        //    var skills = await _utilities.GetSkillsAsync(items);
        //    var platform = await _utilities.GetGamePlatformAsync(gameViewModel.GamePlatform);
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromForm] AddGameViewModel gameViewModel)
        {
            try
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

                    // Получаем из БД выбранные скиллы и платформу
                    List<string>? selectedSkills = JsonSerializer.Deserialize<List<string>>(gameViewModel.SkillsLearning);
                    List<SkillLearning> skills = await _utilities.GetSkillsLearningAsync(selectedSkills);
                    var platform = await _utilities.GetGamePlatformAsync(gameViewModel.GamePlatform);
                    var languageLvl = await _utilities.GetLanguageLevelAsync(gameViewModel.LanguageLevel);

                    string? authorId = _userManager.GetUserId(User);

                    if (gameViewModel.UploadedGame != null) // Если файл загруженной игры не пустой, то она десктоп
                    {
                        Game newGame = new Game
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId,
                            ImagesURLs = imagesPaths,
                            PublicationDate = DateTime.Now,
                            LastUpdateDate = DateTime.Now,
                            GamePlatform = platform,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl
                        };
                        await _gamesRepository.AddAsync(newGame); // После выполнения этой строки код выбрасывает с ошибкой

                        // Теперь можно использовать ID для сохранения файла
                        string? gameUrl = await _fileProvider.SafeFileAsync(gameViewModel.UploadedGame, newGame.Id, newGame.Title);

                        // Если нужно обновить URL игры после сохранения файла
                        if (!string.IsNullOrEmpty(gameUrl))
                        {
                            newGame.GameURL = gameUrl;
                            await _databaseContext.SaveChangesAsync(); // Сохраняем изменения
                        }

                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return Json(new { success = true, redirectUrl = Url.Action("Profile", "Profile", new { userId = authorId }) });
                    }
                    else // Инача - игра веб
                    {
                        Game newGame = new Game
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId,
                            ImagesURLs = imagesPaths,
                            PublicationDate = DateTime.Now,
                            LastUpdateDate = DateTime.Now,
                            GamePlatform = platform,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl,
                            GameURL = gameViewModel.GameURL
                        };

                        await _gamesRepository.AddAsync(newGame);

                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return RedirectToAction("Profile", "Profile", new { userId = authorId });
                    }
                }

                // Нужно чтобы отобразить список скиллов из БД для игры
                ViewBag.SkillsLearning = _databaseContext.SkillsLearning.Select(type => type.Name);
                return View(gameViewModel);
            }
            catch (Exception ex)
            {
                // Нужно сделать логирование
                return StatusCode(500, new { error = ex.Message });
            }

        }
    }
}
