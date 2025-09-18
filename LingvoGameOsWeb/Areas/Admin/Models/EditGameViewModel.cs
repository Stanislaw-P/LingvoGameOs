using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Areas.Admin.Models
{
    public class EditGameViewModel
    {
        public int Id { get; set; }
        //[Required(ErrorMessage ="Обязательное поле")]
        //[MaxLength(21, ErrorMessage ="Максимальная длина 21 символ")]
        //[MinLength(4, ErrorMessage ="Минимальная длина 4 символа")]
        public string Title { get; set; } = null!;

        //[Required(ErrorMessage = "Обязательное поле")]
        //[MaxLength(200, ErrorMessage = "Максимальная длина 200 символов")]
        //[MinLength(10, ErrorMessage = "Минимальная длина 10 символов")]
        public string Description { get; set; } = null!;

        //[Required(ErrorMessage = "Обязательное поле")]
        //[MaxLength(550, ErrorMessage = "Максимальная длина 550 символов")]
        //[MinLength(10, ErrorMessage = "Минимальная длина 10 символов")]
        public string Rules { get; set; } = null!;

        //[Required(ErrorMessage = "Обязательное поле")]
        public string LanguageLevel { get; set; } = null!;

        //[Required(ErrorMessage = "Обязательное поле")]
        public List<string> SkillsLearning { get; set; }

        public string? CurrentCoverImagePath { get; set; }
        //[Required(ErrorMessage = "Обязательное поле")]
        public IFormFile CoverImage { get; set; } = null!;
        public FileInfo? CoverImageInfo { get; set; }

        public List<ImageFileInfo> ImagesFilesInfo { get; set; } = null!;
        //[Required(ErrorMessage = "Обязательное поле")]
        public IFormFile[]? UploadedImages { get; set; }
        public List<string>? DeletedImages { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public string VideoUrl { get; set; } = null!;

        public string? CurrentGameURL { get; set; }
        public string? GameURL { get; set; }
        public FileInfo? GameFileInfo { get; set; }

        //[Required(ErrorMessage = "Обязательное поле")]
        public string? GameFolderName { get; set; }
        //[Required(ErrorMessage = "Обязательное поле")]
        public int Port { get; set; }
        //[Required(ErrorMessage = "Обязательное поле")]
        public string GamePlatform { get; set; } = null!;
        public DateTimeOffset DispatchDate { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public User Author { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public string? LastMessage { get; set; }
    }
}
