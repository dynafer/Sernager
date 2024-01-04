using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Services;

internal static class ManageEnvironmentFlow
{
    internal const string NAME = "ManageEnvironments";

    internal static void Run()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
                .Select(x => (x, x))
                .ToArray();

            IBasePlugin plugin = new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options)
                .AddOptions(
                    ("Add a group", "AddGroup"),
                    ("Remove a group(s)", "RemoveGroups")
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
                    case "AddGroup":
                        AddGroup();
                        break;
                    case "RemoveGroups":
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
            return new InputPlugin()
                .SetPrompt("Enter an environment group name without white spaces (Cancel: Empty input)")
                .UseValidator((string input) =>
                {
                    input = input.Replace(" ", string.Empty);

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return true;
                    }

                    if (Program.Service.GetEnvironmentGroupNames().Contains(input))
                    {
                        return false;
                    }

                    return true;
                });
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string input = ((string)result).Replace(" ", string.Empty);

            if (!string.IsNullOrWhiteSpace(input))
            {
                Program.Service.ManageEnvironmentGroup(input);
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow("AddGroup", pluginHandler, resultHandler);
    }

    internal static void RemoveGroups()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
                .Select(x => (x, x))
                .ToArray();

            IBasePlugin plugin = new MultiSelectionPlugin<string>()
                .SetPrompt("Choose a group(s) to remove (Cancel: No selection):")
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
                Program.Service.ManageEnvironmentGroup(group).RemoveGroup();
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow("RemoveGroups", pluginHandler, resultHandler);
    }
}
