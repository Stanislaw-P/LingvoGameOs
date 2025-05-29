using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class UserGame
    {
        public string UserId { get; set; } // Первичный ключ пользователя
        public User User { get; set; }

        public int GameId { get; set; } // Первичный ключ игры
        public Game Game { get; set; }

        public DateTime LastLaunch { get; set; }
        public double AssignedRating { get; set; }
        public int PointsReceived { get; set; }
    }
}
