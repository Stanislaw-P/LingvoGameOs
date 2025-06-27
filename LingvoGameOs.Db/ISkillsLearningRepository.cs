
using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Db
{
    public interface ISkillsLearningRepository
    {
        Task<List<SkillLearning>> GetAllAsync();
        Task<List<SkillLearning>> GetExistingSkillsAsync(List<string> namesSelectedSkills);
    }
}