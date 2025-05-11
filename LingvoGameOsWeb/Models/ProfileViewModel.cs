using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string Surname { get; set; }

        public string Email { get; set; }

        public int Level { get; set; }

        public string Role { get; set; }

        public string Description { get; set; }
    }
}
