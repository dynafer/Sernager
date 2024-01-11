using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Resources;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Extensions;

internal static class FlowPromptPluginExxtension
{
    internal static TPlugin AddFlowCommonSelectionOptions<TPlugin>(this TPlugin plugin)
        where TPlugin : ListBasePlugin<string>
    {
        plugin.AddOptions(
            (ResourceRetriever.Shared.GetString("Back"), "Back"),
            (ResourceRetriever.Shared.GetString("Home"), "Home"),
            (ResourceRetriever.Shared.GetString("Exit"), "Exit")
        );

        return plugin;
    }

    internal static TPlugin AddFlowCommonSelectionOptions<TPlugin, TOptionValue>(this TPlugin plugin, TOptionValue backOption, TOptionValue homeOption, TOptionValue exitOption)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        plugin.AddOptions(
            (ResourceRetriever.Shared.GetString("Back"), backOption),
            (ResourceRetriever.Shared.GetString("Home"), homeOption),
            (ResourceRetriever.Shared.GetString("Exit"), exitOption)
        );

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"{FlowManager.CommonResourcePack.GetString("Name")}: {manager.CurrentGroup.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.ShortName))
        {
            description.Add($"{FlowManager.CommonResourcePack.GetString("ShortName")}: {manager.CurrentGroup.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.Description))
        {
            description.Add($"{FlowManager.CommonResourcePack.GetString("Description")}: {manager.CurrentGroup.Description}");
        }

        description.Add($"{FlowManager.CommonResourcePack.GetString("Path")}: {manager.CreateCommandGroupPath(" > ")}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager, CommandModel commandModel)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"{FlowManager.CommonResourcePack.GetString("Name")}: {commandModel.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
        {
            description.Add($"{FlowManager.CommonResourcePack.GetString("ShortName")}: {commandModel.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(commandModel.Description))
        {
            description.Add($"{FlowManager.CommonResourcePack.GetString("Description")}: {commandModel.Description}");
        }

        description.Add($"{FlowManager.CommonResourcePack.GetString("Path")}: {manager.CreateCommandPath(" > ", commandModel.Name)}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, IEnvironmentManager manager)
        where TPlugin : IBasePlugin
    {
        plugin.AddDescriptions(
            $"{FlowManager.CommonResourcePack.GetString("Name")}: {manager.EnvironmentGroup.Name}",
            $"{FlowManager.CommonResourcePack.GetString("AdditionMode")}: {manager.AdditionMode.GetDescription()}"
        );

        return plugin;
    }
}
