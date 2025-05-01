using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LingvoGameOs.Controllers
{
    public class GameController : Controller
    {
        readonly IGamesRepository gamesRepository;

		public GameController(IGamesRepository gamesRepository)
		{
			this.gamesRepository = gamesRepository;
		}

		public IActionResult Index(int id)
        {
            var existingGame = gamesRepository.TryGetById(id);
            if (existingGame == null)
                return NotFound();
            return View(existingGame);
        }

        public IActionResult Start(int idGame)
        {
            var existingGame = gamesRepository.TryGetById(idGame);

			if (existingGame == null)
				return NotFound();

			// TODO: Нужно придумать что-нибудь с хранением расположения игры и портом
			string runningScript = Path.Combine("/home/stas/games/", "piece-by-piece", "run.sh");

			if (!System.IO.File.Exists(runningScript))
				return NotFound("Game script not found");

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
				return View(existingGame.GameURL);
			}
			catch (Exception ex)
			{
				return BadRequest("Error starting game: " + ex.Message);
			}
		}
    }
}
