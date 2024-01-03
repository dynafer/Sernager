using Sernager.Core.Models;

namespace Sernager.Core.Managers;

public interface IGroupManager
{
    GroupModel CurrentGroup { get; }
    void RemoveMainGroup();
    void RemoveCurrentGroup();
    IGroupManager UseItem(Guid id);
    IGroupManager PrevGroup();
    List<GroupItemModel> GetItems();
}
