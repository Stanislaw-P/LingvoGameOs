using LingvoGameOs.Db;
using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace LingvoGameOs.Helpers
{
    public class Utilities
    {
        readonly DatabaseContext _databaseContext;

        public Utilities(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<SkillLearning>> GetSkillsAsync(List<string> selectedSkills)
        {
            // Получаем соответствующие записи из базы данных
            var resultSkills = await _databaseContext.SkillsLearning
                .Where(skill => selectedSkills.Contains(skill.Name))
                .ToListAsync();

            return resultSkills;
        }
    }
}
