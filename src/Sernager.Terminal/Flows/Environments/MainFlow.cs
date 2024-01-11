using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments;

[Flow(Alias = "Environment")]
internal sealed class MainFlow : IFlow
{
    void IFlow.Prompt()
    {
        (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        string addGroupId = Guid.NewGuid().ToString();
        string removeGroupId = Guid.NewGuid().ToString();

        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt(FlowManager.CommonResourcePack.GetString("ChooseOptionPrompt"))
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
                .AddOptions(
                    (FlowManager.CommonResourcePack.GetString("AddGroup"), addGroupId),
                    (FlowManager.CommonResourcePack.GetString("RemoveGroup"), removeGroupId)
                )
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        if (result == addGroupId)
        {
            FlowManager.RunFlow("Environment.Main.AddGroup");
            return;
        }

        if (result == removeGroupId)
        {
            FlowManager.RunFlow("Environment.Main.RemoveGroup");
            return;
        }

        FlowManager.RunFlow("Environment.Manage", Program.Service.ManageEnvironmentGroup(result));
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
