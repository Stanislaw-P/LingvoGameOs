using LingvoGameOs.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class PlatformsDbRepository : IPlatformsRepository
    {
        readonly DatabaseContext databaseContext;

        public PlatformsDbRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<Platform?> GetExistingPlatformAsync(string nameSelectedPlatform)
        {
            // Получаем соответствующую запись из базы данных
            return await databaseContext.Platforms.FirstOrDefaultAsync(p => p.Name == nameSelectedPlatform);
        }

    }
}
