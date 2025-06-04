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

        public async Task<List<SkillLearning>> GetSkillsAsync(string selectedSkills)
        {
            List<string> selectedSkillNames = selectedSkills.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            // Получаем соответствующие записи из базы данных
            var resultSkills = await _databaseContext.SkillsLearning
                .Where(skill => selectedSkillNames.Contains(skill.Name))
                .ToListAsync();

            return resultSkills;
        }
    }
}
