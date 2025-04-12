using LingvoGameOs.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db
{
    public interface IDevUsersRepository
    {
        List<DevUser> GetAll();
        DevUser TryGetById(int id);
        void Add(DevUser devUser);
        void Remove(DevUser devUser);
    }
}
