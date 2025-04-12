using LingvoGameOs.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public class PlayerUsersDbRepository : IPlayerUsersRepository
    {
        readonly DatabaseContext databaseContext;

        public PlayerUsersDbRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public List<PlayerUser> GetAll()
        {
            return databaseContext.PlayerUsers.ToList();
        }
        public PlayerUser TryGetById(int id)
        {
            return databaseContext.PlayerUsers.FirstOrDefault(playerUser => playerUser.Id == id);
        }
        public void Add(PlayerUser playerUser)
        {
            databaseContext.PlayerUsers.Add(playerUser);
            databaseContext.SaveChanges();
        }
        public void Remove(PlayerUser playerUser)
        {
            databaseContext.PlayerUsers.Remove(playerUser);
            databaseContext.SaveChanges();
        }
    }
}
