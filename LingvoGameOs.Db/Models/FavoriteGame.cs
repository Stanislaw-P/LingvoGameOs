using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class FavoriteGame
    {
        public Guid Id { get; set; }
        
        public User User { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public Game Game { get; set; } = null!;
        public int GameId { get; set; }

        public DateTimeOffset DateAdded { get; set; }
    }
}
