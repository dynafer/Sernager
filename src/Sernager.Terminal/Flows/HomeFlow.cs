using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows;

[Flow]
internal sealed class HomeFlow : IFlow
{
    void IFlow.Prompt()
    {
        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Home"))
                .SetPrompt("Prompt")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptionsUsingResourcePack(
                    ("RunCommand", "RunCommand"),
                    ("ManageCommands", "ManageCommands"),
                    ("ManageEnvironments", "ManageEnvironments"),
                    ("SaveAs", "SaveAs"),
                    ("SaveAsUserFriendly", "SaveAsUserFriendly")
                )
                .AddOption(FlowManager.CommonResourcePack.GetString("Exit"), "Exit")
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
            case "SaveAsUserFriendly":
                FlowManager.RunFlow("SaveAsUserFriendly");
                break;
            default:
                Environment.Exit(0);
                break;
        }
    }

    bool IFlow.TryJump(string _, bool __)
    {
        FlowManager.IsManagementMode = false;
        FlowManager.JumpFlow("Command.Main");

        return true;
    }
}
