using LingvoGameOs.Models;
using LingvoGameOs.Repositories;
using Microsoft.AspNetCore.Mvc;

// Auth: Teacher, Admin only access logic
class GroupController : Controller
{
    GroupRepository groupRepository;

    public GroupController()
    {
        groupRepository = new GroupRepository();
    }

    public void AddPlayer(int groupId, int userId)
    {
        groupRepository.AddPlayer(groupId, userId);
    }

    public void RemovePlayer(int groupId, int userId)
    {
        groupRepository.RemovePlayer(groupId, userId);
    }

    public void UpdateGroup(GroupModel groupModel) { }

    public void CreateGroup(GroupModel newModel)
    {
        groupRepository.CreateGroup(newModel);
    }

    public void DeleteGroup(int groupId)
    {
        groupRepository.DeleteGroup(groupId);
    }

    public GroupModel GetGroup(int id)
    {
        return groupRepository.GetById(id);
    }
}
