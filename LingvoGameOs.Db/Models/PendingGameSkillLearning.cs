namespace LingvoGameOs.Db.Models
{
    public class PendingGameSkillLearning
    {
        public int PendingGameId { get; set; }
        public PendingGame? PendingGame { get; set; }

        public int SkillLearningId { get; set; }
        public SkillLearning? SkillLearning { get; set; }
    }
}
