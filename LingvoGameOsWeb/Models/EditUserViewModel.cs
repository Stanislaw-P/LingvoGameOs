using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string Surname { get; set; }
        public string Description { get; set; }
        public string AvatarImgPath { get; set; }
        public IFormFile UploadedFile { get; set; }
    }
}
