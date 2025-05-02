using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
