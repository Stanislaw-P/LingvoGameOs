using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class SkillsLearningDbRepository : ISkillsLearningRepository
    {
        readonly DatabaseContext databaseContext;

        public SkillsLearningDbRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<List<SkillLearning>> GetAllAsync()
        {
            return await databaseContext.SkillsLearning.ToListAsync();
        }

        public async Task<List<SkillLearning>> GetExistingSkillsAsync(List<string> namesSelectedSkills)
        {
            // Получаем соответствующие записи из базы данных
            var resultSkills = await databaseContext.SkillsLearning
                .Where(skill => namesSelectedSkills.Contains(skill.Name))
                .ToListAsync();

            return resultSkills;
        }
    }
}
