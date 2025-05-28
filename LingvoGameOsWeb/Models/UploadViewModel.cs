using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class UploadViewModel
    {
        [Required(ErrorMessage = "Введите название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите краткое описание")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Введите описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Введите правила")]
        public string Rules { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        public string Category { get; set; }

        public List<string> Keywords { get; set; }

        public string Level { get; set; }

        [Required(ErrorMessage = "Выберите платформу")]
        public string Platform { get; set; }

        [Required(ErrorMessage = "Выберите обложку игры")]
        public string CoverImageURL { get; set; }

        public string VideoURL { get; set; }

        public List<string> ScreenshotsURL { get; set; }

    }
}
