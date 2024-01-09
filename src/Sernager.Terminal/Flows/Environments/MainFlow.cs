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

        string addEnvironmentGroupId = Guid.NewGuid().ToString();
        string removeEnvironmentGroupId = Guid.NewGuid().ToString();

        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt("Choose a group or an option:")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
                .AddOptions(
                    ("Add a group", addEnvironmentGroupId),
                    ("Remove a group(s)", removeEnvironmentGroupId)
                )
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        if (result == addEnvironmentGroupId)
        {
            FlowManager.RunFlow("Environment.Main.AddGroup");
            return;
        }

        if (result == removeEnvironmentGroupId)
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
