using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Areas.Admin.Models
{
    public class AdminViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Description { get; set; }
        public string? AvatarImgPath { get; set; }
        public List<Game>? ExistingDevGames { get; set; }
        public List<PendingGame>? PendingGames { get; set; }
    }
}
