namespace LingvoGameOs.Models;

class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Image { get; set; }
    public required string Link { get; set; }
}

class Teacher : User
{
    public required GroupModel[] Groups { get; set; }
}

class Player : User
{
    public required GroupModel[] Groups { get; set; }
}

class Game
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }
    public required string Link { get; set; }
}

class GroupModel
{
    public int Id { get; set; }
    public required Teacher Teacher { get; set; }
    public required Player[] Players { get; set; }
    public required Game Game { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }
    public required string Link { get; set; }
}
