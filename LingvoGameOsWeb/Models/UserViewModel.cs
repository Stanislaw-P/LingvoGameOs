using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public List<Game>? PlayerGames { get; set; }
        public List<Game>? DevGames { get; set; }
    }
}
