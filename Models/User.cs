namespace LingvoGameOs.Models
{
    public abstract class User
    {
        static int instanceCounter = 0;
        int Id { get; }
        string Email { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        string Surname { get; set; }
        string Description { get; set; }
        string ImagePath { get; set; }

        public User(string email, string password, string name, string surname)
        {
            Id = instanceCounter;
            Email = email;
            Password = password;
            Name = name;
            Surname = surname;
            instanceCounter++;
        }
        public override string ToString()
        {
            return $"Id: {Id}\nEmail: {Email}\nPassword: {Password}\nName: {Name}\nSurname: {Surname}";
        }
    }
}
