namespace LingvoGameOs.Models
{
    public abstract class User
    {
        Guid Id { get; }
        string Email { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        string Surname { get; set; }
        string Description { get; set; }
        string ImagePath { get; set; }

        public User(string email, string password, string name, string surname)
        {
            Id = Guid.NewGuid();
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
        }
        public override string ToString()
        {
            return $"Email: {Email}\nPassword: {Password}\nName: {Name}\nSurname: {Surname}";
        }
    }
}
