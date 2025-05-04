//using LingvoGameOs.Db;
//using LingvoGameOs.Db.Models;
//using Microsoft.EntityFrameworkCore;

//namespace LingvoGameOs.Services
//{
//    public class UserGameService
//    {
//        private readonly DatabaseContext databaseContext;
//        private readonly IdentityContext identityContext;

//        public UserGameService(DatabaseContext databaseContext, IdentityContext identityContext)
//        {
//            this.databaseContext = databaseContext;
//            this.identityContext = identityContext;
//        }

//        // я не знаю что это, но пусть пока лежит здесь
//        public async Task<List<Game>> GetGamesByUserIdAsync(string userId)
//        {
//            var userExists = await identityContext.Users.AnyAsync(x => x.Id == userId);
//            if (userExists)
//            {
//                return await databaseContext.Games.Where(x => x.AuthorStringId == userId).ToListAsync();
//            }
//            return new List<Game>();
//        }
//    }
//}
