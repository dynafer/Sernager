using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage")]
internal sealed class RemoveItemFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal RemoveItemFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        (string, Guid)[] options = mManager.GetItems()
            .Select(item =>
            {
                string itemString = $"{item.GetNameString()} ({item.GetTypeString()})";

                return (itemString, item.Id);
            })
            .ToArray();

        Guid[] selectedItems = Prompter.Prompt(
            new MultiSelectionPlugin<Guid>()
                .SetPrompt(FlowManager.GetResourceString("Command", "SelectSubgroupOrCommandToRemovePromptWithCancel"))
                .AddFlowDescriptions(mManager)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options)
        ).ToArray();

        foreach (Guid id in selectedItems)
        {
            mManager.RemoveItem(id);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
