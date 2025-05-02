using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
