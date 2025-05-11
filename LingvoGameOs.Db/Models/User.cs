using Microsoft.AspNetCore.Identity;

namespace LingvoGameOs.Db.Models
{
	public class User : IdentityUser
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public int Level { get; set; } = 1;
		public string Description { get; set; }
	}
}
