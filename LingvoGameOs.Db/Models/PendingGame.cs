using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class PendingGame
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [MaxLength(201)]
        public string Description { get; set; }
        public string Rules { get; set; }
        public DateTime DispatchDate { get; set; }
        public User Author { get; set; }
        public string AuthorId { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public int LanguageLevelId { get; set; }
        public List<SkillLearning> SkillsLearning { get; set; }
        public string? CoverImagePath { get; set; }
        public List<string>? ImagesPaths { get; set; }
        public string? VideoUrl { get; set; }
        public string? GameURL { get; set; }
        public string? GameFolderName { get; set; }
        public Platform GamePlatform { get; set; }
        public int GamePlatformId { get; set; }
        public string? LastMessage { get; set; }
    }
}
