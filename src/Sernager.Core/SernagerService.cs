using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    ICommandManager ISernagerService.ManageCommandGroup(string groupName, string shortName, string description)
    {
        ICommandManager manager;

        if (CacheManager.TryGet($"Command-Group-{groupName}", out manager))
        {
            return manager;
        }

        return new CommandManager(groupName, shortName, description);
    }

    IEnvironmentManager ISernagerService.ManageEnvironmentGroup(string groupName)
    {
        IEnvironmentManager manager;

        if (CacheManager.TryGet($"Environment-Group-{groupName}", out manager))
        {
            return manager;
        }

        return new EnvironmentManager(groupName);
    }

    string[] ISernagerService.GetCommandGroupNames()
    {
        return Configurator.Config.CommandMainGroups.Keys.ToArray();
    }

    string[] ISernagerService.GetCommandGroupShortNames()
    {
        return Configurator.Config.CommandMainGroups.Values.Select(x => x.ShortName).ToArray();
    }

    string[] ISernagerService.GetEnvironmentGroupNames()
    {
        return Configurator.Config.EnvironmentGroups.Keys.ToArray();
    }

    void ISernagerService.SaveAs(EConfigurationType type)
    {
        Configurator.SaveAsFile(type);
    }
}
