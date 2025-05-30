using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    class GameSkillLearning
    {
		public int GameId { get; set; }
		public Game? Game { get; set; }

		public int GameTypeId { get; set; }
		public SkillLearning? GameType { get; set; }
	}
}
