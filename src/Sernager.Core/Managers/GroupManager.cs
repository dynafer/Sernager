using Sernager.Core.Configs;
using Sernager.Core.Models;

namespace Sernager.Core.Managers;

internal sealed class GroupManager : IGroupManager
{
    private string mName;
    private GroupModel mMainGroup;
    private Stack<Guid> mParents = new Stack<Guid>();
    public GroupModel CurrentGroup { get; private set; }

    internal GroupManager(string name)
    {
        if (!Configurator.Config.Groups.ContainsKey(name))
        {
            Configurator.Config.Groups.Add(name, new GroupModel());
        }

        mName = name;
        mMainGroup = Configurator.Config.Groups[name];
        CurrentGroup = mMainGroup;
    }

    public void RemoveMainGroup()
    {
        removeItems(mMainGroup);

        mMainGroup.Items.Clear();
        mMainGroup = null!;
        CurrentGroup = null!;
        Configurator.Config.Groups.Remove(mName);
        mName = null!;
    }

    public void RemoveCurrentGroup()
    {
        if (CurrentGroup == mMainGroup)
        {
            RemoveMainGroup();
            return;
        }

        Guid currentId = mParents.Pop();

        removeItems(CurrentGroup);

        Guid id = mParents.Peek();
        CurrentGroup = Configurator.Config.SubGroups[id];

        CurrentGroup.Items.Remove(currentId);
    }

    public IGroupManager UseItem(Guid id)
    {
        if (!CurrentGroup.Items.Contains(id))
        {
            ExceptionManager.Throw<SernagerException>($"Item not found in group. Id: {id}");
            return this;
        }

        if (mParents.Contains(id))
        {
            ExceptionManager.Throw<SernagerException>($"Circular reference detected. Id: {id}");
            return this;
        }

        if (Configurator.Config.SubGroups.ContainsKey(id))
        {
            mParents.Push(id);
            CurrentGroup = Configurator.Config.SubGroups[id];
        }
        else if (Configurator.Config.Commands.ContainsKey(id))
        {
            // FIX ME: Execute command
        }
        else
        {
            ExceptionManager.Throw<SernagerException>($"Item not found. Id: {id}");
        }

        return this;
    }

    public IGroupManager PrevGroup()
    {
        if (mParents.Count == 0)
        {
            CurrentGroup = mMainGroup;

            return this;
        }

        mParents.Pop();

        Guid id = mParents.Peek();
        CurrentGroup = Configurator.Config.SubGroups[id];

        return this;
    }

    public List<GroupItemModel> GetItems()
    {
        List<GroupItemModel> items = new List<GroupItemModel>();

        foreach (Guid id in CurrentGroup.Items)
        {
            if (Configurator.Config.SubGroups.ContainsKey(id))
            {
                items.Add(new GroupItemModel()
                {
                    Id = id,
                    Item = Configurator.Config.SubGroups[id]
                });
            }
            else if (Configurator.Config.Commands.ContainsKey(id))
            {
                items.Add(new GroupItemModel()
                {
                    Id = id,
                    Item = Configurator.Config.Commands[id]
                });
            }
            else
            {
                ExceptionManager.Throw<SernagerException>($"Item not found. Id: {id}");
            }
        }

        return items;
    }

    private void removeItems(GroupModel model)
    {
        foreach (Guid id in model.Items)
        {
            if (Configurator.Config.SubGroups.ContainsKey(id))
            {
                removeItems(Configurator.Config.SubGroups[id]);
            }
            else if (Configurator.Config.Commands.ContainsKey(id))
            {
                Configurator.Config.Commands.Remove(id);
            }
            else
            {
                ExceptionManager.Throw<SernagerException>($"Item not found. Id: {id}");
            }
        }
    }
}
