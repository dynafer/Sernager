using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    IEnvironmentManager ISernagerService.ManageEnvironmentGroup(string groupName, string shortName, string description)
    {
        IEnvironmentManager manager;

        if (CacheManager.TryGet($"Environment-Group-{groupName}", out manager))
        {
            return manager;
        }

        return new EnvironmentManager(groupName, shortName, description);
    }

    ICommandManager ISernagerService.ManageCommandGroup(string groupName, string shortName, string description)
    {
        ICommandManager manager;

        if (CacheManager.TryGet($"Command-Group-{groupName}", out manager))
        {
            return manager;
        }

        return new CommandManager(groupName, shortName, description);
    }

    string[] ISernagerService.GetCommandGroupNames()
    {
        return Configurator.Config.CommandMainGroups.Keys.ToArray();
    }

    void ISernagerService.SaveAs(EConfigurationType type)
    {
        Configurator.SaveAsFile(type);
    }
}
