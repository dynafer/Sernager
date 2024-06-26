using Sernager.Core.Configs;
using Sernager.Core.Models;

namespace Sernager.Core.Managers;

internal sealed class CommandManager : ICommandManager
{
    private readonly Stack<Guid> mParents = new Stack<Guid>();
    public GroupModel MainGroup { get; private set; }
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

        MainGroup = Configurator.Config.CommandMainGroups[name];
        CurrentGroup = MainGroup;
    }

    public void RemoveMainGroup()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return;
        }

        CurrentGroup = null!;

        removeItems(MainGroup);

        mParents.Clear();

        MainGroup.Items.Clear();
        Configurator.Config.CommandMainGroups.Remove(MainGroup.Name);
        CacheManager.Remove($"Command-Group-{MainGroup.Name}");
        MainGroup = null!;
    }

    public void RemoveCurrentGroup()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return;
        }

        if (CurrentGroup == MainGroup)
        {
            RemoveMainGroup();
            return;
        }

        removeItems(CurrentGroup);

        Guid currentId = mParents.Pop();

        Configurator.Config.CommandSubgroups.Remove(currentId);

        if (mParents.Count == 0)
        {
            CurrentGroup = MainGroup;
            CurrentGroup.Items.Remove(currentId);
            return;
        }

        Guid id = mParents.Peek();
        CurrentGroup = Configurator.Config.CommandSubgroups[id];
        CurrentGroup.Items.Remove(currentId);
    }

    public void RemoveItem(Guid Id)
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return;
        }

        if (!CurrentGroup.Items.Contains(Id))
        {
            ExceptionManager.Throw<SernagerException>($"Item not found in group. Id: {Id}");
            return;
        }

        if (Configurator.Config.CommandSubgroups.ContainsKey(Id))
        {
            removeItems(Configurator.Config.CommandSubgroups[Id]);
            Configurator.Config.CommandSubgroups.Remove(Id);
        }
        else if (Configurator.Config.Commands.ContainsKey(Id))
        {
            Configurator.Config.Commands.Remove(Id);
        }
        else
        {
            ExceptionManager.Throw<SernagerException>($"Item not found. Id: {Id}");
        }

        CurrentGroup.Items.Remove(Id);
    }

    public ICommandManager UseItem(Guid id)
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return this;
        }

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

        if (Configurator.Config.CommandSubgroups.ContainsKey(id))
        {
            mParents.Push(id);
            CurrentGroup = Configurator.Config.CommandSubgroups[id];
        }
        else if (Configurator.Config.Commands.ContainsKey(id))
        {
            return this;
        }
        else
        {
            ExceptionManager.Throw<SernagerException>($"Item not found. Id: {id}");
        }

        return this;
    }

    public string[] GetBreadcrumb()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return Array.Empty<string>();
        }

        List<string> path = [
            MainGroup.Name
        ];

        foreach (Guid id in mParents)
        {
            path.Add(Configurator.Config.CommandSubgroups[id].Name);
        }

        return path.ToArray();
    }

    public ICommandManager PrevGroup()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return this;
        }

        if (mParents.Count <= 1)
        {
            mParents.Clear();
            CurrentGroup = MainGroup;

            return this;
        }

        mParents.Pop();

        Guid id = mParents.Peek();
        CurrentGroup = Configurator.Config.CommandSubgroups[id];

        return this;
    }

    public ICommandManager GoMainGroup()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return this;
        }

        mParents.Clear();
        CurrentGroup = MainGroup;

        return this;
    }

    public GroupModel GetPrevGroup()
    {
        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return null!;
        }

        if (mParents.Count <= 1)
        {
            return MainGroup;
        }

        Guid id = mParents.ElementAt(mParents.Count - 1);

        return Configurator.Config.CommandSubgroups[id];
    }

    public List<GroupItemModel> GetItems()
    {
        List<GroupItemModel> items = new List<GroupItemModel>();

        if (MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return items;
        }

        foreach (Guid id in CurrentGroup.Items)
        {
            if (Configurator.Config.CommandSubgroups.ContainsKey(id))
            {
                items.Add(new GroupItemModel()
                {
                    Id = id,
                    Item = Configurator.Config.CommandSubgroups[id]
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
            if (Configurator.Config.CommandSubgroups.ContainsKey(id))
            {
                removeItems(Configurator.Config.CommandSubgroups[id]);
                Configurator.Config.CommandSubgroups.Remove(id);
            }
            else if (Configurator.Config.Commands.ContainsKey(id))
            {
                Configurator.Config.Commands.Remove(id);
                CacheManager.Remove($"Executor-{id}");
            }
            else
            {
                ExceptionManager.Throw<SernagerException>($"Item not found. Id: {id}");
            }
        }
    }
}
