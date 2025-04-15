using System.Security.AccessControl;
using System;
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db.Models
{
	public class Game
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DevUser Author { get; set; }
		public int AuthorId { get; set; }
		public DateTime PublicationDate { get; set; }
		public DateTime LastUpdateDate { get; set; }
		public LanguageLevel LanguageLevel { get; set; }
		public int LanguageLevelId { get; set; }
		public List<GameType> GameTypes { get; set; }
		public double Raiting { get; set; }
		public string CoverImageURL { get; set; }
		public string GameURL { get; set; }
		public Platform GamePlatform { get; set; }
		public int GamePlatformId { get; set; }
		public int NumberDownloads { get; set; }
	}
}
