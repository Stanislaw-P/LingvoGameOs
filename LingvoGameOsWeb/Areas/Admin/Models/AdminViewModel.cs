using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
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
        public string AvatarImgUrl { get; set; } = null!;
        public List<GameViewModel>? ExistingDevGames { get; set; }
        public List<PendingGameViewModel>? PendingGames { get; set; }
        public List<GameViewModel>? InactiveGames { get; set; }
        public int NumberDevelopers { get; set; }
    }
}
