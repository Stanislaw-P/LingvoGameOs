using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class AddGameViewModel
    {
        [Required(ErrorMessage = "Название - обязательное поле")]
        [MaxLength(25, ErrorMessage = "Максимальная длина названия 25 символов")]
        [MinLength(4, ErrorMessage = "Минимальная длина названия 4 символа")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Описание -обязательное поле")]
        [MaxLength(200, ErrorMessage = "Максимальная длина описания 200 символов")]
        [MinLength(10, ErrorMessage = "Минимальная длина описания 10 символов")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Правила - обязательное поле")]
        [MaxLength(550, ErrorMessage = "Максимальная длина 550 символов")]
        [MinLength(10, ErrorMessage = "Минимальная длина 10 символов")]
        public string Rules { get; set; } = null!;

        [Required(ErrorMessage = "Уровень языка - обязательное поле")]
        public string LanguageLevel { get; set; } = null!;

        [Required(ErrorMessage = "Выберите хотя бы один развиваемый навык")]
        public string SkillsLearning { get; set; } = null!;

        [Required(ErrorMessage = "Загрузите обложку игры")]
        public IFormFile CoverImage { get; set; } = null!;

        [Required(ErrorMessage = "Загрузите хотя бы один скриншот игры")]
        public IFormFile[] UploadedImages { get; set; } = null!;

        public string? VideoUrl { get; set; }

        [Required(ErrorMessage = "Ссылка на GitHub - обязательное поле")]
        public string GameGitHubUrl { get; set; } = null!;
        public IFormFile? UploadedGameFile { get; set; }

        [Required(ErrorMessage = "Выберите платформу")]
        public string GamePlatform { get; set; } = null!;
    }
}