using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class RunCommandFlow
{
    internal static void Run()
    {
        (string, string)[] options = Program.Service.GetGroupNames()
            .Select(x => (x, x))
            .ToArray();

        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose a group or command:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(options)
            .AddOptions(
                ("Back", "Back"),
                ("Exit", "Exit")
            );

        HistoryResultHandler handler = (object result) =>
        {
            switch (result)
            {
                case "Back":
                    HistoryManager.Prev();
                    break;
                case "Exit":
                    Environment.Exit(0);
                    break;
                default:
                    RunCommand((string)result);
                    break;
            }
        };

        FlowManager.RunFlow("RunCommand", plugin, handler);
    }

    internal static void RunCommand(string command)
    {
        // FIX ME: Implement this flow
    }
}
