using Sernager.Core.Options;
using Sernager.Terminal.Flows.Services;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class ManageServiceFlow
{
    internal static void Run()
    {
        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose an option:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(
                ("Manage environments", "ManageEnvironments"),
                ("Manage commands", "ManageCommands"),
                ("Save as ...", "SaveAs")
            )
            .AddFlowCommonOptions();

        HistoryResultHandler handler = (object result) =>
        {
            switch (result)
            {
                case "ManageEnvironments":
                    ManageEnvironmentFlow.Run();
                    break;
                case "ManageCommands":
                    ManageCommandFlow.Run();
                    break;
                case "SaveAs":
                    SaveAs();
                    break;
                default:
                    FlowManager.TryHandleCommonOptions((string)result);
                    break;
            }
        };

        FlowManager.RunFlow("ManageServices", plugin, handler);
    }

    internal static void SaveAs()
    {
        (string, string)[] options = Enum.GetNames<EConfigurationType>()
            .Select(x => (x, x))
            .ToArray();

        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose a configuration type:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(options)
            .AddFlowCommonOptions();

        HistoryResultHandler handler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                EConfigurationType type = Enum.Parse<EConfigurationType>((string)result);
                Program.Service.SaveAs(type);
                HistoryManager.Prev();
            }
        };

        FlowManager.RunFlow("SaveAs", plugin, handler);
    }
}
