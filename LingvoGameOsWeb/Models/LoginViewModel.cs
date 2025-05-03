using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 100 символов")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
