using Sernager.Core.Models;

namespace Sernager.Core.Managers;

public interface ICommandManager
{
    GroupModel MainGroup { get; }
    GroupModel CurrentGroup { get; }
    void RemoveMainGroup();
    void RemoveCurrentGroup();
    void RemoveItem(Guid Id);
    CommandModel GetCommand(Guid id);
    ICommandManager UseItem(Guid id);
    string[] GetBreadcrumb();
    ICommandManager PrevGroup();
    ICommandManager GoMainGroup();
    GroupModel GetPrevGroup();
    List<GroupItemModel> GetItems();
}
