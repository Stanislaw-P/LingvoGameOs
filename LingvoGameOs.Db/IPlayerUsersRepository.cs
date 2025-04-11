using LingvoGameOs.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public interface IPlayerUsersRepository
    {
        List<PlayerUser> GetAll();
        PlayerUser TryGetById(int id);
        void Add(PlayerUser playerUser);
        void Remove(PlayerUser playerUser);
    }
}
