using Sernager.Core.Models;

namespace Sernager.Core.Managers;

public interface ICommandManager
{
    GroupModel CurrentGroup { get; }
    void RemoveMainGroup();
    void RemoveCurrentGroup();
    ICommandManager UseItem(Guid id);
    ICommandManager PrevGroup();
    List<GroupItemModel> GetItems();
}
