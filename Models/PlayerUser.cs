namespace LingvoGameOs.Models
{
    public class PlayerUser : User
    {
        public readonly string Role = "Игрок";
        int Level { get; set; } = 1;
        public PlayerUser(string email, string password, string name, string surname) : base(email, password, name, surname)
        {
        }
    }
}
