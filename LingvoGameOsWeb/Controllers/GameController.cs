using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LingvoGameOs.Controllers
{
    public class GameController : Controller
    {
        readonly IGamesRepository gamesRepository;
        readonly UserManager<User> _userManager;

        public GameController(IGamesRepository gamesRepository, UserManager<User> userManager)
        {
            this.gamesRepository = gamesRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> DetailsAsync(int idGame)
        {
            var existingGame = await gamesRepository.TryGetByIdAsync(idGame);
            if (existingGame == null)
                return NotFound();

            return View(existingGame);
        }

        public async Task<IActionResult> StartAsync(int idGame)
        {
            var existingGame = await gamesRepository.TryGetByIdAsync(idGame);

            if (existingGame == null)
                return NotFound();

            ViewBag.GameId = idGame;
            ViewBag.GameTitle = existingGame.Title;


            // TODO: Нужно придумать что-нибудь с хранением расположения игры и портом
            string runningScript = Path.Combine("/home/stas/games/", "piece-by-piece", "run.sh");

            if (!System.IO.File.Exists(runningScript))
                return View();

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
                    await gamesRepository.AddPlayerToGameHistoryAsync(existingGame, currentUser);
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest("Error starting game: " + ex.Message);
            }
        }
    }
}
