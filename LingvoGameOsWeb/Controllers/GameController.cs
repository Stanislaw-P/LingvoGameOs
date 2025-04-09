using LingvoGameOs.Db;
using Microsoft.AspNetCore.Mvc;

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
    }
}
