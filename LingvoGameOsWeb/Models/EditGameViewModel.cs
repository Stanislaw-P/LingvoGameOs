using LingvoGameOs.Areas.Admin.Models;
using LingvoGameOs.Db.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class EditGameViewModel
    {
        public int Id { get; set; }
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

        public string LanguageLevel { get; set; } = null!;

        public List<string>? SkillsLearning { get; set; }

        public string? CurrentCoverImagePath { get; set; }
        public IFormFile? CoverImage { get; set; }
        public FileInfo? CoverImageInfo { get; set; }

        public List<ImageFileInfo>? ImagesFilesInfo { get; set; }
        public IFormFile[]? UploadedImages { get; set; }
        public List<string>? DeletedImages { get; set; }

        public string? VideoUrl { get; set; }

        public string? CurrentGameFilePath { get; set; }
        public string? GameFilePath { get; set; }
        public string GameGitHubUrl { get; set; } = null!;
        public FileInfo? GameFileInfo { get; set; }

        public string? GameFolderName { get; set; }
        public int Port { get; set; }
        public string GamePlatform { get; set; } = null!;
        public DateTimeOffset DispatchDate { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public User? Author { get; set; }
        public string AuthorId { get; set; } = null!;
        public string? LastMessage { get; set; }
    }
}
