using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string UserName { get; set; }
    }
}
