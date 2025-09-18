﻿using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 100 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 100 символов")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
