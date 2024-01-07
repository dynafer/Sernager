using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage", Name = "RemoveItem")]
internal sealed class RemoveItemFlow : IFlow
{
    private ICommandManager mManager;

    internal RemoveItemFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        (string, Guid)[] options = mManager.GetItems()
            .Select(x =>
            {
                string type = x.Item switch
                {
                    CommandModel => "Command",
                    GroupModel => "Group",
                    _ => "Unknown"
                };

                string name = x.Item switch
                {
                    CommandModel command => command.Name,
                    GroupModel group => group.Name,
                    _ => "Unknown"
                };

                name += $" ({type})";

                return (name, x.Id);
            })
            .ToArray();

        Guid[] selectedItems = Prompter.Prompt(
            new MultiSelectionPlugin<Guid>()
                .SetPrompt("Select a group or a command to remove (Cancel: No selection):")
                .AddFlowDescriptions(mManager)
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options)
        ).ToArray();

        foreach (Guid id in selectedItems)
        {
            mManager.RemoveItem(id);
        }

        FlowManager.RunPreviousFlow();
    }
}
