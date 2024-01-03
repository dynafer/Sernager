using Sernager.Core.Options;
using Sernager.Terminal.Flows.Services;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal static class ManageServicesFlow
{
    internal static void Run()
    {
        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose an option:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(
                ("Manage settings", "ManageSettings"),
                ("Manage groups", "ManageGroups"),
                ("Save as ...", "SaveAs"),
                ("Back", "Back"),
                ("Exit", "Exit")
            );

        HistoryResultHandler handler = (object result) =>
        {
            switch (result)
            {
                case "ManageSettings":
                    ManageSettingsFlow.Run();
                    break;
                case "ManageGroups":
                    ManageGroupsFlow.Run();
                    break;
                case "SaveAs":
                    SaveAs();
                    break;
                case "Back":
                    HistoryManager.Prev();
                    break;
                default:
                    Environment.Exit(0);
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
                    EConfigurationType type = Enum.Parse<EConfigurationType>((string)result);
                    Program.Service.SaveAs(type);
                    HistoryManager.Prev();
                    break;
            }
        };

        FlowManager.RunFlow("SaveAs", plugin, handler);
    }
}
