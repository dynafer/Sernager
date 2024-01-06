using Sernager.Core.Managers;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Helpers;

internal static class ManageFlowHelper
{
    internal static string CreateCommandGroupPath(this ICommandManager manager, string delimiter)
    {
        return string.Join(delimiter, manager.GetPath());
    }

    internal static string CreateCommandPath(this ICommandManager manager, string delimiter, string commandName)
    {
        return $"{manager.CreateCommandGroupPath(delimiter)}{delimiter}{commandName}";
    }

    internal static string CreateCommandGroupFlowName(this ICommandManager manager, string prefix)
    {
        return $"{manager.CreateCommandGroupPath(".")}.{prefix}";
    }

    internal static string CreateCommandGroupFlowName(this ICommandManager manager, string prefix, string flowName)
    {
        return $"{manager.CreateCommandGroupFlowName(prefix)}.{flowName}";
    }

    internal static string CreateCommandFlowName(this ICommandManager manager, string prefix, string commandName)
    {
        return $"{manager.CreateCommandPath(".", commandName)}.{prefix}";
    }

    internal static string CreateCommandFlowName(this ICommandManager manager, string prefix, string commandName, string flowName)
    {
        return $"{manager.CreateCommandFlowName(prefix, commandName)}.{flowName}";
    }

    internal static ITypePlugin<string> CreateDescriptionPromptPlugin(string description, bool bSkip)
    {
        string promptSkipText = bSkip ? " (Skip: Empty input)" : "";

        return new InputPlugin()
            .SetPrompt($"Enter a description for the command group{promptSkipText}")
            .SetInitialInput(description);
    }
}
