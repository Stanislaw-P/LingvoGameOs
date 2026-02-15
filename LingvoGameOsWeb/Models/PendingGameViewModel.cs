using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Models
{
    public class PendingGameViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Rules { get; set; } = null!;
        public DateTimeOffset DispatchDate { get; set; }
        public DateTimeOffset LastUpdateDate { get; set; }
        public User? Author { get; set; }
        public LanguageLevel? LanguageLevel { get; set; }
        public List<SkillLearning>? SkillsLearning { get; set; }
        public string? CoverImagePath { get; set; }
        public List<string>? ImagesPaths { get; set; }
        public string? VideoUrl { get; set; }
        public string? GameFilePath { get; set; }
        public string GameGitHubUrl { get; set; } = null!;
        public Platform? GamePlatform { get; set; }
        public string? LastMessage { get; set; }
    }
}
