﻿using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using LingvoGameOs.Helpers;
using LingvoGameOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace LingvoGameOs.Controllers
{
    public class GameController : Controller
    {
        readonly IGamesRepository _gamesRepository;
        readonly UserManager<User> _userManager;
        readonly ISkillsLearningRepository _skillsLearningRepository;
        readonly IPlatformsRepository _platformsRepository;
        readonly ILanguageLevelsRepository _languageLevelsRepository;
        readonly FileProvider _fileProvider;
        readonly IPendingGamesRepository _pendingGamesRepository;

        public GameController(IGamesRepository gamesRepository, UserManager<User> userManager, ISkillsLearningRepository skillsLearningRepository, IWebHostEnvironment appEnvironment, IPlatformsRepository platformsRepository, ILanguageLevelsRepository languageLevelsRepository, IPendingGamesRepository pendingGamesRepository)
        {
            _gamesRepository = gamesRepository;
            _userManager = userManager;
            _skillsLearningRepository = skillsLearningRepository;
            _fileProvider = new FileProvider(appEnvironment);
            _platformsRepository = platformsRepository;
            _languageLevelsRepository = languageLevelsRepository;
            _pendingGamesRepository = pendingGamesRepository;
        }

        public async Task<IActionResult> DetailsAsync(int idGame)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(idGame);
            if (existingGame == null)
                return NotFound();

            return View(existingGame);
        }

        public async Task<IActionResult> StartAsync(int idGame)
        {
            var existingGame = await _gamesRepository.TryGetByIdAsync(idGame);

            if (existingGame == null)
                return NotFound();

            ViewBag.GameId = idGame;
            ViewBag.GameTitle = existingGame.Title;

            if (existingGame.GameFolderName == null)
                existingGame.GameFolderName = "temp";

            // TODO: Нужно придумать что-нибудь с хранением расположения игры и портом
            string runningScript = Path.Combine(Constants.GameFolderPath, existingGame.GameFolderName, "run.sh");

            if (!System.IO.File.Exists(runningScript))
            {
                ViewBag.GameUrl = null;
                return View();
            }

            var runningProcess = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = runningScript,
                WorkingDirectory = Path.GetDirectoryName(runningScript),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                Process.Start(runningProcess);
                ViewBag.GameUrl = existingGame.GameURL;

                // Добавление игры в историю пользователя
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                    await _gamesRepository.AddPlayerToGameHistoryAsync(existingGame, currentUser);
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest("Error starting game: " + ex.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> UploadAsync()
        {
            var skillLearnings = await _skillsLearningRepository.GetAllAsync();
            ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadAsync([FromForm] AddGameViewModel gameViewModel)
        {
            try
            {
                //if (gameViewModel.UploadedGame == null && gameViewModel.GameURL == null)
                //{
                //    ModelState.AddModelError("", "Необходимо добавить ссылку на игру, либо загрузить файл игры");
                //    return View(gameViewModel);
                //}

                //if (gameViewModel.UploadedGame != null && gameViewModel.GameURL != null)
                //{
                //    ModelState.AddModelError("", "Можно выбрать только одну платформу для игры");
                //    return View(gameViewModel);
                //}

                if (ModelState.IsValid)
                {
                    string? coverImagePath = await _fileProvider.SafeFileAsync(gameViewModel.CoverImage, ImageFolders.Games);
                    List<string> imagesPaths = await _fileProvider.SafeImagesFilesAsync(gameViewModel.UploadedImages, ImageFolders.Games);
                    
                    // Получаем из БД выбранные скиллы и платформу
                    List<string>? selectedSkills = JsonSerializer.Deserialize<List<string>>(gameViewModel.SkillsLearning);
                    List<SkillLearning> skills = await _skillsLearningRepository.GetExistingSkillsAsync(selectedSkills);
                    var platform = await _platformsRepository.GetExistingPlatformAsync(gameViewModel.GamePlatform);
                    var languageLvl = await _languageLevelsRepository.GetExistingLanguageLevelAsync(gameViewModel.LanguageLevel);

                    string? authorId = _userManager.GetUserId(User);

                    if (gameViewModel.UploadedGame != null) // Если файл загруженной игры не пустой, то она десктоп
                    {
                        PendingGame pendingGame = new PendingGame
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId!,
                            CoverImagePath = coverImagePath!,
                            ImagesPaths = imagesPaths,
                            DispatchDate = DateTime.Now,
                            GamePlatform = platform!,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl!,
                            VideoUrl = gameViewModel.VideoUrl
                        };
                        await _pendingGamesRepository.AddAsync(pendingGame);
                        // Теперь можно использовать ID для сохранения файла
                        string? gameUrl = await _fileProvider.SafeGameFileAsync(gameViewModel.UploadedGame, pendingGame.Id, pendingGame.Title, GameFolders.PendingGame);

                        // Если нужно обновить URL игры после сохранения файла
                        if (!string.IsNullOrEmpty(gameUrl))
                        {
                            await _pendingGamesRepository.ChangeGameUrl(gameUrl, pendingGame);
                        }

                        //Game newGame = new Game
                        //{
                        //    Title = gameViewModel.Title,
                        //    Description = gameViewModel.Description,
                        //    Rules = gameViewModel.Rules,
                        //    AuthorId = authorId!,
                        //    CoverImagePath = coverImagePath!,
                        //    ImagesPaths = imagesPaths,
                        //    PublicationDate = DateTime.Now,
                        //    LastUpdateDate = DateTime.Now,
                        //    GamePlatform = platform!,
                        //    SkillsLearning = skills,
                        //    LanguageLevel = languageLvl!,
                        //    VideoUrl = gameViewModel.VideoUrl
                        //};
                        //await _gamesRepository.AddAsync(newGame); 

                        //// Теперь можно использовать ID для сохранения файла
                        //string? gameUrl = await _fileProvider.SafeFileAsync(gameViewModel.UploadedGame, newGame.Id, newGame.Title);

                        //// Если нужно обновить URL игры после сохранения файла
                        //if (!string.IsNullOrEmpty(gameUrl))
                        //{
                        //    await _gamesRepository.ChangeGameUrl(gameUrl, newGame);
                        //}

                        // смена роли игрока на разработчика
                        await ChangeRolePlayerToDevAsync();
                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Profile", new { userId = authorId }) });
                    }
                    else // Инача - игра веб
                    {
                        PendingGame pendingGame = new PendingGame
                        {
                            Title = gameViewModel.Title,
                            Description = gameViewModel.Description,
                            Rules = gameViewModel.Rules,
                            AuthorId = authorId!,
                            CoverImagePath = coverImagePath!,
                            ImagesPaths = imagesPaths,
                            DispatchDate = DateTime.Now,
                            GamePlatform = platform!,
                            SkillsLearning = skills,
                            LanguageLevel = languageLvl!,
                            GameURL = gameViewModel.GameURL!,
                            VideoUrl = gameViewModel.VideoUrl
                        };
                        await _pendingGamesRepository.AddAsync(pendingGame);
                        //Game newGame = new Game
                        //{
                        //    Title = gameViewModel.Title,
                        //    Description = gameViewModel.Description,
                        //    Rules = gameViewModel.Rules,
                        //    AuthorId = authorId!,
                        //    CoverImagePath = coverImagePath!,
                        //    ImagesPaths = imagesPaths,
                        //    PublicationDate = DateTime.Now,
                        //    LastUpdateDate = DateTime.Now,
                        //    GamePlatform = platform!,
                        //    SkillsLearning = skills,
                        //    LanguageLevel = languageLvl!,
                        //    GameURL = gameViewModel.GameURL!,
                        //    VideoUrl = gameViewModel.VideoUrl
                        //};

                        //await _gamesRepository.AddAsync(newGame);

                        // смена роли игрока на разработчика
                        await ChangeRolePlayerToDevAsync();
                        //Нужно как-нибудь оповестить пользователя об успешной заргузки игры
                        return RedirectToAction("Index", "Profile", new { userId = authorId });
                    }
                }

                // Нужно чтобы отобразить список скиллов из БД для игры
                var skillLearnings = await _skillsLearningRepository.GetAllAsync();
                ViewBag.SkillsLearning = skillLearnings.Select(sl => sl.Name);
                return View(gameViewModel);
            }
            catch (Exception ex)
            {
                // Нужно сделать логирование
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // смена роли игрока на разработчика
        private async Task ChangeRolePlayerToDevAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // если есть роль игрока, то удаляю ее
                if (await _userManager.IsInRoleAsync(currentUser, Constants.PlayerRoleName))
                {
                    await _userManager.RemoveFromRoleAsync(currentUser, Constants.PlayerRoleName);
                }
                // если нет роли разработчика, то добавляю ее
                if (!await _userManager.IsInRoleAsync(currentUser, Constants.DevRoleName))
                {
                    await _userManager.AddToRoleAsync(currentUser, Constants.DevRoleName);
                }
            }
        }
    }
}
