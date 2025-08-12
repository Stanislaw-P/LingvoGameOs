using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Areas.Admin.Models
{
    public class EditGameViewModel
    {
        public int Id { get; set; }
        //[Required(ErrorMessage ="Обязательное поле")]
        //[MaxLength(21, ErrorMessage ="Максимальная длина 21 символ")]
        //[MinLength(4, ErrorMessage ="Минимальная длина 4 символа")]
        public string Title { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        //[MaxLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        //[MinLength(10, ErrorMessage = "Минимальная длина 10 символов")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        //[MaxLength(550, ErrorMessage = "Максимальная длина 550 символов")]
        //[MinLength(10, ErrorMessage = "Минимальная длина 10 символов")]
        public string Rules { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public string LanguageLevel { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public List<string> SkillsLearning { get; set; }

        public string CurrentCoverImage { get; set; }
        //[Required(ErrorMessage = "Обязательное поле")]
        public IFormFile CoverImage { get; set; }

        public List<string> CurrentImagesPaths { get; set; }
        //[Required(ErrorMessage = "Обязательное поле")]
        public IFormFile[] UploadedImages { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public string VideoUrl { get; set; }

        public string? GameURL { get; set; }
        public IFormFile? UploadedGame { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public string GamePlatform { get; set; }
        public DateTime DispatchDate { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public User Author { get; set; }
    }
}
