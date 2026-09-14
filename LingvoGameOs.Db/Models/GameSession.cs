using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int GameId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool Used { get; set; } 
    }
}
