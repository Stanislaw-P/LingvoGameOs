using System.Collections.Generic;
using LingvoGameOs.Models;

namespace LingvoGameOs.Repositories;

class GroupRepository
{
    static PlayerRepository playerRepository = new();
    static List<GroupModel> groups =
    [
        new GroupModel(
            0,
            new Teacher(
                0,
                "Teacher",
                "teacher@example.com",
                "securepassword",
                "default-image.png",
                "https://example.com/teacher"
            ),
            [],
            new Game(0, "Game", "Description", "default-image.png", "https://example.com/game"),
            "Group",
            "",
            "",
            ""
        ),
    ];

    public void CreateGroup(GroupModel groupModel)
    {
        groups.Add(groupModel);
    }

    public void DeleteGroup(int groupId)
    {
        GroupModel group = this.GetById(groupId);

        groups.Remove(group);
    }

    public void AddPlayer(int groupId, int userId)
    {
        GroupModel group = this.GetById(groupId);
        Player user = playerRepository.GetById(userId);

        group.Players.Add(user);

        // return success message;
    }

    public void RemovePlayer(int groupId, int userId)
    {
        GroupModel group = this.GetById(groupId);
        Player user = playerRepository.GetById(userId);

        group.Players.Remove(user);
    }

    public void UpdateGroup(GroupModel newModel) { }

    public GroupModel GetById(int id)
    {
        GroupModel? group = groups.FirstOrDefault(x => x.Id == id);

        if (group == null)
            throw new Exception("Group not found");

        return group;
    }
}
