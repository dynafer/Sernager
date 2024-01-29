using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using System.Diagnostics;

namespace Sernager.Core.Extensions;

public static class CommandManagerExtension
{
    public static ICommandManager AddSubgroup(this ICommandManager manager, string groupName, string shortName, string description)
    {
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return manager;
        }

        Debug.Assert(CanUseName(manager, groupName, true), "Suppoed to be checked before.");

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
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return manager;
        }

        Debug.Assert(CanUseName(manager, commandModel.Name, true), "Suppoed to be checked before.");
        Debug.Assert(
            string.IsNullOrWhiteSpace(commandModel.ShortName) || CanUseName(manager, commandModel.ShortName, true),
            "Suppoed to be checked before."
        );

        Guid id = Guid.NewGuid();

        Configurator.Config.Commands.Add(id, commandModel);

        manager.CurrentGroup.Items.Add(id);

        return manager;
    }

    public static bool TryChangeCurrentGroupName(this ICommandManager manager, string name)
    {
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return false;
        }

        Debug.Assert(CanUseName(manager, name, false), "Suppoed to be checked before.");

        if (manager.MainGroup == manager.CurrentGroup)
        {
            Configurator.Config.CommandMainGroups.Remove(manager.MainGroup.Name);
            Configurator.Config.CommandMainGroups.Add(name, manager.MainGroup);
        }

        manager.CurrentGroup.Name = name;

        return true;
    }

    public static bool TryChangeCurrentGroupShortName(this ICommandManager manager, string shortName)
    {
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return false;
        }

        Debug.Assert(CanUseName(manager, shortName, false), "Suppoed to be checked before.");

        manager.CurrentGroup.ShortName = shortName;

        return true;
    }

    public static void ChangeCurrentGroupDescription(this ICommandManager manager, string description)
    {
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return;
        }

        manager.CurrentGroup.Description = description;
    }

    public static bool CanUseName(this ICommandManager manager, string name, bool bCheckSubItems)
    {
        if (manager.MainGroup == null)
        {
            ExceptionManager.Throw<SernagerException>("The main group already removed.");
            return false;
        }

        if (!bCheckSubItems)
        {
            GroupModel prevGroup = manager.GetPrevGroup();

            bool bUsable = manager.CurrentGroup.Name != name;

            if (manager.CurrentGroup == manager.MainGroup)
            {
                bUsable &= !Configurator.Config.CommandMainGroups
                    .Where(x => x.Key == name || x.Value.Name == name || x.Value.ShortName == name)
                    .Any();
            }
            else
            {
                bUsable &= !prevGroup.Items
                    .Where(Configurator.Config.CommandSubgroups.ContainsKey)
                    .Select(x => Configurator.Config.CommandSubgroups[x])
                    .Where(x => x.Name == name || x.ShortName == name)
                    .Any();
            }

            bUsable &= !prevGroup.Items
                .Where(Configurator.Config.Commands.ContainsKey)
                .Select(x => Configurator.Config.Commands[x])
                .Where(x => x.Name == name || x.ShortName == name)
                .Any();

            return bUsable;
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
