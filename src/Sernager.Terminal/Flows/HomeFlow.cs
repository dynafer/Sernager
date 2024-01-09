using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

internal sealed class HomeFlow : IFlow
{
    void IFlow.Prompt()
    {
        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(
                    ("Run command", "RunCommand"),
                    ("Manage commands", "ManageCommands"),
                    ("Manage environments", "ManageEnvironments"),
                    ("Save as ...", "SaveAs"),
                    ("Exit", "Exit")
                )
        );

        switch (result)
        {
            case "RunCommand":
                FlowManager.IsManagementMode = false;
                FlowManager.RunFlow("Command.Main");
                break;
            case "ManageCommands":
                FlowManager.IsManagementMode = true;
                FlowManager.RunFlow("Command.Main");
                break;
            case "ManageEnvironments":
                FlowManager.IsManagementMode = true;
                FlowManager.RunFlow("Environment.Main");
                break;
            case "SaveAs":
                FlowManager.RunFlow("SaveAs");
                break;
            default:
                Environment.Exit(0);
                break;
        }
    }
}