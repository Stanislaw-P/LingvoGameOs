using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    class GameGameType
    {
		public int GameId { get; set; }
		public Game? Game { get; set; }

		public int GameTypeId { get; set; }
		public GameType? GameType { get; set; }
	}
}
