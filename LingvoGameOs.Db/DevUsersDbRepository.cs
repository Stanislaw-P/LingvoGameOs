using LingvoGameOs.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class DevUsersDbRepository : IDevUsersRepository
    {
        readonly DatabaseContext databaseContext;

        public DevUsersDbRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public List<DevUser> GetAll()
        {
            return databaseContext.DevUsers.ToList();
        }
        public DevUser TryGetById(int id)
        {
            return databaseContext.DevUsers.FirstOrDefault(devUser => devUser.Id == id);
        }
        public void Add(DevUser devUser)
        {
            databaseContext.DevUsers.Add(devUser);
            databaseContext.SaveChanges();
        }
        public void Remove(DevUser devUser)
        {
            databaseContext.DevUsers.Remove(devUser);
            databaseContext.SaveChanges();
        }
    }
}
