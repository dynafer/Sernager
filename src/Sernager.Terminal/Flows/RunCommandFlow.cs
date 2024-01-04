using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class RunCommandFlow
{
    internal static void Run()
    {
        (string, string)[] options = Program.Service.GetCommandGroupNames()
            .Select(x => (x, x))
            .ToArray();

        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose a group or command:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(options)
            .AddFlowCommonOptions();

        HistoryResultHandler handler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                RunCommand((string)result);
            }
        };

        FlowManager.RunFlow("RunCommand", plugin, handler);
    }

    internal static void RunCommand(string command)
    {
        // FIX ME: Implement this flow
    }
}
