using Sernager.Terminal.Flows;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Managers;

internal static class FlowManager
{
    private static readonly Dictionary<string, Guid> mHistoryIds = new Dictionary<string, Guid>();

    internal static void Start(string[] commands)
    {
        Home(commands.Length > 0);
    }

    internal static void Home(bool bSkip = false)
    {
        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose an option:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(
                ("Run command", "RunCommand"),
                ("Manage services", "ManageServices"),
                ("Exit", "Exit")
            );

        HistoryResultHandler handler = (object result) =>
        {
            switch (result)
            {
                case "RunCommand":
                    RunCommandFlow.Run();
                    break;
                case "ManageServices":
                    ManageServicesFlow.Run();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        };

        RunFlow("Home", plugin, handler, bSkip);
    }

    internal static void RunFlow(string key, IBasePlugin plugin, HistoryResultHandler handler, bool bSkip = false)
    {
        if (!mHistoryIds.ContainsKey(key))
        {
            HistoryModel model = new HistoryModel(plugin, handler);

            mHistoryIds.Add(key, model.Id);

            HistoryManager.Run(model, bSkip);
        }
        else
        {
            HistoryManager.Run(mHistoryIds[key], bSkip);
        }
    }
}
