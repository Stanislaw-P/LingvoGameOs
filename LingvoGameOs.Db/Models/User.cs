using Microsoft.AspNetCore.Identity;

namespace LingvoGameOs.Db.Models
{
	public class User : IdentityUser
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
	}
}
