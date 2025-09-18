using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public User Author { get; set; } = null!;
        public string AuthorId { get; set; }
        public int Rating { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public bool IsApproved { get; set; }
    }
}
