using Sernager.Core.Models;

namespace Sernager.Core.Managers;

public interface ICommandManager
{
    GroupModel MainGroup { get; }
    GroupModel CurrentGroup { get; }
    void RemoveMainGroup();
    void RemoveCurrentGroup();
    void RemoveItem(Guid Id);
    bool IsCommand(Guid id);
    CommandModel GetCommand(Guid id);
    ICommandManager UseItem(Guid id);
    string[] GetPath();
    ICommandManager PrevGroup();
    GroupModel GetPrevGroup();
    List<GroupItemModel> GetItems();
}
