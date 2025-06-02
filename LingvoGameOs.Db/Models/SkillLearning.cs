namespace LingvoGameOs.Db.Models
{
	public class SkillLearning
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public List<Game> Games { get; set; }
	}
}