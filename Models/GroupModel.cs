namespace LingvoGameOs.Models;

class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Image { get; set; }
    public string Link { get; set; }

    public User(int id, string name, string email, string password, string image, string link)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Image = image;
        Link = link;
    }
}

class Teacher : User
{
    public List<GroupModel> Groups { get; set; }

    public Teacher(int id, string name, string email, string password, string image, string link)
        : base(id, name, email, password, image, link)
    {
        Groups = [];
    }
}

class Player : User
{
    public List<GroupModel> Groups { get; set; }

    public Player(int id, string name, string email, string password, string image, string link)
        : base(id, name, email, password, image, link)
    {
        Groups = [];
    }
}

class Game
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Link { get; set; }

    public Game(int id, string name, string description, string image, string link)
    {
        Id = id;
        Name = name;
        Description = description;
        Image = image;
        Link = link;
    }
}

class GroupModel
{
    public int Id { get; set; }
    public Teacher Teacher { get; set; }
    public List<Player> Players { get; set; }
    public Game Game { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Link { get; set; }

    public GroupModel(
        int id,
        Teacher teacher,
        List<Player> players,
        Game game,
        string name,
        string description,
        string image,
        string link
    )
    {
        Id = id;
        Teacher = teacher;
        Players = players;
        Game = game;
        Name = name;
        Description = description;
        Image = image;
        Link = link;
    }
}
