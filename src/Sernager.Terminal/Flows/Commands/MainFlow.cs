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
        List<(string, string)> options = Program.Service.GetCommandGroupNames()
            .Select(x => (x, x))
            .ToList();

        string addGroupId = Guid.NewGuid().ToString();
        string removeGroupId = Guid.NewGuid().ToString();

        if (FlowManager.IsManagementMode)
        {
            options.AddRange([
                (FlowManager.CommonResourcePack.GetString("AddGroup"), addGroupId),
                (FlowManager.CommonResourcePack.GetString("RemoveGroup"), removeGroupId)
            ]);
        }

        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt(FlowManager.IsManagementMode ? "MainManagePrompt" : "MainRunPrompt")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options.ToArray())
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        if (result == addGroupId)
        {
            FlowManager.RunFlow("Command.Main.AddGroup");
            return;
        }

        if (result == removeGroupId)
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
