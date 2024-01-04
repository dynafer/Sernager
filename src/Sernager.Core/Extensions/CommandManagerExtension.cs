using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;

namespace Sernager.Core.Extensions;

public static class CommandManagerExtension
{
    public static ICommandManager AddGroup(this ICommandManager manager, GroupModel groupModel)
    {
        if (existsInGroup(groupModel))
        {
            ExceptionManager.Throw<SernagerException>($"Group {groupModel.Name} already exists.");
            return manager;
        }

        Guid id = Guid.NewGuid();

        Configurator.Config.CommandSubGroups.Add(id, groupModel);

        manager.CurrentGroup.Items.Add(id);

        return manager;
    }

    public static ICommandManager AddCommand(this ICommandManager manager, CommandModel commandModel)
    {
        if (existsInCommands(commandModel))
        {
            ExceptionManager.Throw<SernagerException>($"Command {commandModel.Name} already exists.");
            return manager;
        }

        Guid id = Guid.NewGuid();

        Configurator.Config.Commands.Add(id, commandModel);

        manager.CurrentGroup.Items.Add(id);

        return manager;
    }

    private static bool existsInGroup(GroupModel groupModel)
    {
        return Configurator.Config.CommandMainGroups.Values.Where(x => x == groupModel).FirstOrDefault() != null ||
               Configurator.Config.CommandSubGroups.Values.Where(x => x == groupModel).FirstOrDefault() != null;
    }

    private static bool existsInCommands(CommandModel commandModel)
    {
        return Configurator.Config.Commands.Values.Where(x => x == commandModel).FirstOrDefault() != null;
    }
}
