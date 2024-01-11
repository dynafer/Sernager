using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments.Mains;

[Flow(Alias = "Environment.Main")]
internal sealed class RemoveGroupFlow : IFlow
{
    void IFlow.Prompt()
    {
        (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        IEnumerable<string> selectedGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .SetPrompt(FlowManager.CommonResourcePack.GetString("SelectGroupToRemoveWithCancel"))
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
        );

        foreach (string group in selectedGroups)
        {
            Program.Service.ManageEnvironmentGroup(group).RemoveGroup();
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
