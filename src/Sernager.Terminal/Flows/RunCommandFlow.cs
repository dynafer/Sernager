using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class RunCommandFlow
{
    internal const string NAME = "RunCommand";

    internal static void Run(bool bSkip = false)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
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

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                RunCommand((string)result);
            }
        };

        FlowManager.RunFlow(NAME, pluginHandler, resultHandler, bSkip);
    }

    internal static void RunCommand(string command)
    {
        // FIX ME: Implement this flow
    }
}
