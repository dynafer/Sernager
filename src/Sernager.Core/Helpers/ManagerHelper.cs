using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;

namespace Sernager.Core.Helpers;

public static class ManagerHelper
{
    public static bool CanUseCommandMainGroupName(string name)
    {
        return !Configurator.Config.CommandMainGroups.ContainsKey(name) &&
               !Configurator.Config.CommandMainGroups.Values.Where(x => x.ShortName == name).Any();
    }

    public static bool CanUseEnvironmentGroupName(string name)
    {
        return !Configurator.Config.EnvironmentGroups.ContainsKey(name);
    }

    public static string? GetCommadMainGroupNameOrNull(string nameOrShortName)
    {
        return Configurator.Config.CommandMainGroups.Values
            .Where(x => x.Name == nameOrShortName || x.ShortName == nameOrShortName)
            .Select(x => x.Name)
            .FirstOrDefault();
    }

    public static bool IsCommand(Guid id)
    {
        return Configurator.Config.Commands.ContainsKey(id);
    }

    public static CommandModel? GetCommandOrNull(Guid id)
    {
        if (!Configurator.Config.Commands.ContainsKey(id))
        {
            ExceptionManager.Throw<SernagerException>($"Command not found. Id: {id}");
            return null;
        }

        return Configurator.Config.Commands[id];
    }
}
