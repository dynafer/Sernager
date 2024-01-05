using Sernager.Core.Configs;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Core.Models;

namespace Sernager.Core.Extensions;

public static class CommandManagerExtension
{
    public static ICommandManager AddSubgroup(this ICommandManager manager, string groupName, string shortName, string description)
    {
        GroupModel groupModel = new GroupModel
        {
            Name = groupName,
            ShortName = shortName,
            Description = description
        };

        Guid id = Guid.NewGuid();

        Configurator.Config.CommandSubgroups.Add(id, groupModel);

        manager.CurrentGroup.Items.Add(id);

        return manager;
    }

    public static ICommandManager AddCommand(this ICommandManager manager, CommandModel commandModel)
    {
        Guid id = Guid.NewGuid();

        Configurator.Config.Commands.Add(id, commandModel);

        manager.CurrentGroup.Items.Add(id);

        return manager;
    }

    public static bool ChangeCurrentGroupName(this ICommandManager manager, string name)
    {
        if (!ManagerHelper.CanUseCommandGroupName(name))
        {
            return false;
        }

        if (manager.MainGroup == manager.CurrentGroup)
        {
            Configurator.Config.CommandMainGroups.Remove(manager.MainGroup.Name);
            Configurator.Config.CommandMainGroups.Add(name, manager.MainGroup);
        }

        manager.CurrentGroup.Name = name;

        return true;
    }

    public static bool ChangeCurrentGroupShortName(this ICommandManager manager, string shortName)
    {
        if (!ManagerHelper.CanUseCommandGroupName(shortName))
        {
            return false;
        }

        manager.CurrentGroup.ShortName = shortName;

        return true;
    }

    public static void ChangeCurrentGroupDescription(this ICommandManager manager, string description)
    {
        manager.CurrentGroup.Description = description;
    }

    public static bool CanUseName(this ICommandManager manager, string name, bool bCheckPrevious)
    {
        if (bCheckPrevious)
        {
            GroupModel prevGroup = manager.GetPrevGroup();

            return manager.CurrentGroup.Name != name &&
                   !prevGroup.Items
                       .Where(Configurator.Config.CommandSubgroups.ContainsKey)
                       .Select(x => Configurator.Config.CommandSubgroups[x])
                       .Where(x => x.Name == name || x.ShortName == name)
                       .Any() &&
                   !prevGroup.Items
                       .Where(Configurator.Config.Commands.ContainsKey)
                       .Select(x => Configurator.Config.Commands[x])
                       .Where(x => x.Name == name || x.ShortName == name)
                       .Any();
        }
        else
        {
            return !manager.CurrentGroup.Items
                       .Where(Configurator.Config.CommandSubgroups.ContainsKey)
                       .Select(x => Configurator.Config.CommandSubgroups[x])
                       .Where(x => x.Name == name || x.ShortName == name)
                       .Any() &&
                   !manager.CurrentGroup.Items
                       .Where(Configurator.Config.Commands.ContainsKey)
                       .Select(x => Configurator.Config.Commands[x])
                       .Where(x => x.Name == name || x.ShortName == name)
                       .Any();
        }
    }
}
