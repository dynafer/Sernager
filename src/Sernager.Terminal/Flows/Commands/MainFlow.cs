using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands;

[Flow(Alias = "Command", Name = "Main")]
public class MainFlow : IFlow
{
    void IFlow.Prompt()
    {
        string prompt = "Choose a group:";
        List<(string, string)> options = Program.Service.GetCommandGroupNames()
            .Select(x => (x, x))
            .ToList();

        if (FlowManager.IsManagementMode)
        {
            prompt = "Choose a group or an option:";
            options.AddRange([
                ("Add a group", "AddCommandGroup"),
                ("Remove a group(s)", "RemoveCommandGroup")
            ]);
        }

        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt(prompt)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options.ToArray())
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        switch (result)
        {
            case "AddCommandGroup":
                FlowManager.RunFlow("Command.Main.AddGroup");
                break;
            case "RemoveCommandGroup":
                FlowManager.RunFlow("Command.Main.RemoveGroup");
                break;
            default:
                FlowManager.RunFlow("Command.CurrentGroup", Program.Service.ManageCommandGroup(result));
                break;
        }
    }
}
