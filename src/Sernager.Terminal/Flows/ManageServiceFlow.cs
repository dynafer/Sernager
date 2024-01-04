using Sernager.Core.Options;
using Sernager.Terminal.Flows.Services;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class ManageServiceFlow
{
    internal const string NAME = "ManageServices";

    internal static void Run()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(
                    ("Manage environments", ManageEnvironmentFlow.NAME),
                    ("Manage commands", ManageCommandFlow.NAME),
                    ("Save as ...", "SaveAs")
                )
                .AddFlowCommonOptions();
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            switch (result)
            {
                case ManageEnvironmentFlow.NAME:
                    ManageEnvironmentFlow.Run();
                    break;
                case ManageCommandFlow.NAME:
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

        FlowManager.RunFlow(NAME, pluginHandler, resultHandler);
    }

    internal static void SaveAs()
    {
        HistoryPromptPluginHandler pluginHandler = () =>
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

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                EConfigurationType type = Enum.Parse<EConfigurationType>((string)result);
                Program.Service.SaveAs(type);
                HistoryManager.Prev();
            }
        };

        FlowManager.RunFlow("SaveAs", pluginHandler, resultHandler);
    }
}
