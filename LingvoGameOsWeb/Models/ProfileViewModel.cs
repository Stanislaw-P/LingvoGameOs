using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
    }
}
