using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
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
            (FlowManager.GetResourceString("Common", "Back"), "Back"),
            (FlowManager.GetResourceString("Common", "Home"), "Home"),
            (FlowManager.GetResourceString("Common", "Exit"), "Exit")
        );

        return plugin;
    }

    internal static TPlugin AddFlowCommonSelectionOptions<TPlugin, TOptionValue>(this TPlugin plugin, TOptionValue backOption, TOptionValue homeOption, TOptionValue exitOption)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        plugin.AddOptions(
            (FlowManager.GetResourceString("Common", "Back"), backOption),
            (FlowManager.GetResourceString("Common", "Home"), homeOption),
            (FlowManager.GetResourceString("Common", "Exit"), exitOption)
        );

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"{FlowManager.GetResourceString("Common", "Name")}: {manager.CurrentGroup.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.ShortName))
        {
            description.Add($"{FlowManager.GetResourceString("Common", "ShortName")}: {manager.CurrentGroup.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.Description))
        {
            description.Add($"{FlowManager.GetResourceString("Common", "Description")}: {manager.CurrentGroup.Description}");
        }

        description.Add($"{FlowManager.GetResourceString("Common", "Path")}: {manager.CreateCommandGroupPath(" > ")}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager, CommandModel commandModel)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"{FlowManager.GetResourceString("Common", "Name")}: {commandModel.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
        {
            description.Add($"{FlowManager.GetResourceString("Common", "ShortName")}: {commandModel.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(commandModel.Description))
        {
            description.Add($"{FlowManager.GetResourceString("Common", "Description")}: {commandModel.Description}");
        }

        description.Add($"{FlowManager.GetResourceString("Common", "Path")}: {manager.CreateCommandPath(" > ", commandModel.Name)}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, IEnvironmentManager manager)
        where TPlugin : IBasePlugin
    {
        plugin.AddDescriptions(
            $"{FlowManager.GetResourceString("Common", "Name")}: {manager.EnvironmentGroup.Name}",
            $"{FlowManager.GetResourceString("Common", "AdditionMode")}: {manager.AdditionMode.GetDescription()}"
        );

        return plugin;
    }
}
