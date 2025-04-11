namespace LingvoGameOs.Models
{
    public class PlayerUserViewModel : UserViewModel
    {
        public readonly string Role = "Игрок";
        int Level { get; set; } = 1;
        public PlayerUserViewModel(string email, string password, string name, string surname) : base(email, password, name, surname)
        {
        }
    }
}
