using Sernager.Core.Configs;
using Sernager.Core.Models;

namespace Sernager.Core.Managers;

internal sealed class CommandManager : ICommandManager
{
    private GroupModel mMainGroup;
    private Stack<Guid> mParents = new Stack<Guid>();
    public GroupModel CurrentGroup { get; private set; }

    internal CommandManager(string name, string shortName, string description)
    {
        if (!Configurator.Config.CommandMainGroups.ContainsKey(name))
        {
            GroupModel groupModel = new GroupModel
            {
                Name = name,
                ShortName = shortName,
                Description = description
            };

            Configurator.Config.CommandMainGroups.Add(name, groupModel);
        }

        mMainGroup = Configurator.Config.CommandMainGroups[name];
        CurrentGroup = mMainGroup;
    }

    public void RemoveMainGroup()
    {
        CurrentGroup = null!;

        removeItems(mMainGroup);

        mMainGroup.Items.Clear();
        Configurator.Config.CommandMainGroups.Remove(mMainGroup.Name);
        mMainGroup = null!;
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
        CurrentGroup = Configurator.Config.CommandSubGroups[id];

        CurrentGroup.Items.Remove(currentId);
    }

    public ICommandManager UseItem(Guid id)
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

        if (Configurator.Config.CommandSubGroups.ContainsKey(id))
        {
            mParents.Push(id);
            CurrentGroup = Configurator.Config.CommandSubGroups[id];
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

    public ICommandManager PrevGroup()
    {
        if (mParents.Count == 0)
        {
            CurrentGroup = mMainGroup;

            return this;
        }

        mParents.Pop();

        Guid id = mParents.Peek();
        CurrentGroup = Configurator.Config.CommandSubGroups[id];

        return this;
    }

    public List<GroupItemModel> GetItems()
    {
        List<GroupItemModel> items = new List<GroupItemModel>();

        foreach (Guid id in CurrentGroup.Items)
        {
            if (Configurator.Config.CommandSubGroups.ContainsKey(id))
            {
                items.Add(new GroupItemModel()
                {
                    Id = id,
                    Item = Configurator.Config.CommandSubGroups[id]
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
            if (Configurator.Config.CommandSubGroups.ContainsKey(id))
            {
                removeItems(Configurator.Config.CommandSubGroups[id]);
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
