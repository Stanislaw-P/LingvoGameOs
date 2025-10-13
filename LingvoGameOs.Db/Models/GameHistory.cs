using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class GameHistory
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } // Первичный ключ пользователя
        public User User { get; set; }

        public int GameId { get; set; } // Первичный ключ игры
        public Game Game { get; set; }

        public DateTimeOffset LastLaunch { get; set; }
        public int PointsReceived { get; set; }
    }
}
