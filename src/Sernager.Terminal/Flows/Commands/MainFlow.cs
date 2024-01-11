using Sernager.Core.Helpers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands;

[Flow(Alias = "Command")]
public class MainFlow : IFlow
{
    void IFlow.Prompt()
    {
        string prompt = FlowManager.GetResourceString("Command", "MainRunPrompt");
        List<(string, string)> options = Program.Service.GetCommandGroupNames()
            .Select(x => (x, x))
            .ToList();

        string addCommandGroupId = Guid.NewGuid().ToString();
        string removeCommandGroupId = Guid.NewGuid().ToString();

        if (FlowManager.IsManagementMode)
        {
            prompt = FlowManager.GetResourceString("Command", "MainManagePrompt");
            options.AddRange([
                (FlowManager.GetResourceString("Common", "AddGroup"), addCommandGroupId),
                (FlowManager.GetResourceString("Common", "RemoveGroup"), removeCommandGroupId)
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

        if (result == addCommandGroupId)
        {
            FlowManager.RunFlow("Command.Main.AddGroup");
            return;
        }

        if (result == removeCommandGroupId)
        {
            FlowManager.RunFlow("Command.Main.RemoveGroup");
            return;
        }

        FlowManager.RunFlow("Command.CurrentGroup", Program.Service.ManageCommandGroup(result));
    }

    bool IFlow.TryJump(string command, bool _)
    {
        string? groupName = ManagerHelper.GetCommadGroupNameOrNull(command);
        if (groupName == null)
        {
            return false;
        }

        FlowManager.JumpFlow("Command.CurrentGroup", Program.Service.ManageCommandGroup(groupName));
        return true;
    }
}
