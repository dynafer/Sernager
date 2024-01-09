using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Extensions;

internal static class FlowPromptPluginExxtension
{
    internal static TPlugin AddFlowCommonSelectionOptions<TPlugin>(this TPlugin plugin)
        where TPlugin : ListBasePlugin<string>
    {
        plugin.AddOptions(
            ("Back", "Back"),
            ("Home", "Home"),
            ("Exit", "Exit")
        );

        return plugin;
    }

    internal static TPlugin AddFlowCommonSelectionOptions<TPlugin, TOptionValue>(this TPlugin plugin, TOptionValue backOption, TOptionValue homeOption, TOptionValue exitOption)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        plugin.AddOptions(
            ("Back", backOption),
            ("Home", homeOption),
            ("Exit", exitOption)
        );

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"Name: {manager.CurrentGroup.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.ShortName))
        {
            description.Add($"Short name: {manager.CurrentGroup.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.Description))
        {
            description.Add($"Description: {manager.CurrentGroup.Description}");
        }

        description.Add($"Path: {manager.CreateCommandGroupPath(" > ")}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, ICommandManager manager, CommandModel commandModel)
        where TPlugin : IBasePlugin
    {
        List<string> description = [
            $"Name: {commandModel.Name}"
        ];

        if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
        {
            description.Add($"Short name: {commandModel.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(commandModel.Description))
        {
            description.Add($"Description: {commandModel.Description}");
        }

        description.Add($"Path: {manager.CreateCommandPath(" > ", commandModel.Name)}");

        plugin.AddDescriptions(description.ToArray());

        return plugin;
    }

    internal static TPlugin AddFlowDescriptions<TPlugin>(this TPlugin plugin, IEnvironmentManager manager)
        where TPlugin : IBasePlugin
    {
        plugin.AddDescriptions(
            $"Name: {manager.EnvironmentGroup.Name}",
            $"Addition mode: {manager.AdditionMode.GetDescription()}"
        );

        return plugin;
    }
}
