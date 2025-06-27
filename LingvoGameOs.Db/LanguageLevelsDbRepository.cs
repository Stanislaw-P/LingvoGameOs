using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class LanguageLevelsDbRepository : ILanguageLevelsRepository
    {
        readonly DatabaseContext databaseContext;

        public LanguageLevelsDbRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<LanguageLevel?> GetExistingLanguageLevelAsync(string nameSelectedLangLvl)
        {
            // Получаем соответствующую запись из базы данных
            return await databaseContext.LanguageLevels.FirstOrDefaultAsync(x => x.Name == nameSelectedLangLvl);
        }
    }
}
