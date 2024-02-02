using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sernager.Core;

internal sealed class SernagerService : ISernagerService
{
    ICommandManager ISernagerService.ManageCommandGroup(string groupName, string shortName, string description)
    {
        ICommandManager? manager;

        if (CacheManager.TryGet($"Command-Group-{groupName}", out manager))
        {
            return manager;
        }

        manager = new CommandManager(groupName, shortName, description);

        CacheManager.Set($"Command-Group-{groupName}", manager);

        return manager;
    }

    IEnvironmentManager ISernagerService.ManageEnvironmentGroup(string groupName)
    {
        IEnvironmentManager? manager;

        if (CacheManager.TryGet($"Environment-Group-{groupName}", out manager))
        {
            return manager;
        }

        manager = new EnvironmentManager(groupName);

        CacheManager.Set($"Environment-Group-{groupName}", manager);

        return manager;
    }

    IExecutor ISernagerService.GetExecutor(Guid commandId)
    {
        Debug.Assert(Configurator.Config.Commands.ContainsKey(commandId));

        IExecutor? executor;

        if (CacheManager.TryGet($"Executor-{commandId}", out executor))
        {
            return executor;
        }

        executor = new Executor(Configurator.Config.Commands[commandId]);

        CacheManager.Set($"Executor-{commandId}", executor);

        return executor;
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

    [ExcludeFromCodeCoverage]
    void ISernagerService.SaveAs(EConfigurationType type)
    {
        Configurator.SaveAsFile(type);
    }

    [ExcludeFromCodeCoverage]
    void ISernagerService.SaveAs(EUserFriendlyConfigurationType type)
    {
        Configurator.SaveAsFile(type);
    }
}
