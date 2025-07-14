using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Введите логин")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string Surname { get; set; }
        
        public int Level { get; set; }
        public string? Description { get; set; }
        public string? AvatarImgPath { get; set; }
        public List<Game>? PlayerGames { get; set; }
        public List<PlayerGame>? UserGames { get; set; }
        public List<Game>? DevGames { get; set; }
        public bool IsMyProfile { get; set; }
    }
}
