using System.Security.AccessControl;
using System;
using LingvoGameOs.Models;

namespace LingvoGameOs.data.Models
{
	public class Game
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public User Author { get; set; }
		public DateTime PublicationDate { get; set; }
		public DateTime LastUpdateDate { get; set; }
		public LanguageLevel LanguageLevel { get; set; }
		public List<GameType> GameTypes { get; set; }
		public double Raiting { get; set; }
		public string CoverImageURL { get; set; }
		public string GameURL { get; set; }
		public List<Platform> Platforms { get; set; }
		public List<Technology> Technologys { get; set; }
		public int NumberDownloads { get; set; }
	}
}
