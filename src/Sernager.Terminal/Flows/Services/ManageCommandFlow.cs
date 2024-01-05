using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Services;

internal static class ManageCommandFlow
{
    internal const string NAME = "ManageCommands";

    internal static void Run()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            (string, string)[] options = Program.Service.GetCommandGroupNames()
                .Select(x => (x, x))
                .ToArray();

            IBasePlugin plugin = new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options)
                .AddOptions(
                    ("Add a group", $"{NAME}.AddGroup"),
                    ("Remove a group(s)", $"{NAME}.RemoveGroups")
                )
                .AddFlowCommonOptions();

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                switch (result)
                {
                    case $"{NAME}.AddGroup":
                        AddGroup();
                        break;
                    case $"{NAME}.RemoveGroups":
                        RemoveGroups();
                        break;
                    default:
                        // FIX ME: Handle manage selected group
                        break;
                }
            }
        };

        FlowManager.RunFlow(NAME, pluginHandler, resultHandler);
    }

    internal static void AddGroup()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            string[] groupNames = Program.Service.GetCommandGroupNames();
            string[] groupShortNames = Program.Service.GetCommandGroupShortNames();

            return new InputPlugin()
                .SetPrompt("Enter a command group name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string input) => string.IsNullOrWhiteSpace(input),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string input) => !groupNames.Contains(input) && !groupShortNames.Contains(input),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                );
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string groupName = ((string)result).Replace(" ", string.Empty);

            if (string.IsNullOrWhiteSpace(groupName))
            {
                HistoryManager.Prev();
                return;
            }

            string[] groupNames = Program.Service.GetCommandGroupNames();
            string[] groupShortNames = Program.Service.GetCommandGroupShortNames();

            string shortName = promptShortName(groupName);
            string description = promptDescription();

            Program.Service.ManageCommandGroup(groupName, shortName, description);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow($"{NAME}.AddGroup", pluginHandler, resultHandler);
    }

    internal static void RemoveGroups()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            (string, string)[] options = Program.Service.GetCommandGroupNames()
                .Select(x => (x, x))
                .ToArray();

            IBasePlugin plugin = new MultiSelectionPlugin<string>()
                .SetPrompt("Choose a command group(s) to remove (Cancel: No selection):")
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options);

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            IEnumerable<string> selectedGroups = (IEnumerable<string>)result;

            foreach (string group in selectedGroups)
            {
                Program.Service.ManageCommandGroup(group).RemoveMainGroup();
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow($"{NAME}.RemoveGroups", pluginHandler, resultHandler);
    }

    private static string promptShortName(string groupName)
    {
        string[] groupNames = Program.Service.GetCommandGroupNames();
        string[] groupShortNames = Program.Service.GetCommandGroupShortNames();

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a short name for the command group (Skip: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => shortNameInput != groupName,
                            "The name cannot be the same as the group name.",
                            EInputValidatorHandlerType.Default
                        ),
                        (
                            (string shortNameInput) => !groupNames.Contains(shortNameInput) && !groupShortNames.Contains(shortNameInput),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        return shortName;
    }

    private static string promptDescription()
    {
        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a description for the command group (Skip: Empty input)")
        );

        return description;
    }
}
