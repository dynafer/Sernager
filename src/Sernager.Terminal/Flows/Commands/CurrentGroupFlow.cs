using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands;

[Flow(Alias = "Command")]
internal sealed class CurrentGroupFlow : IFlow
{
    private ICommandManager mManager;
    private Dictionary<string, Guid> mFixedOptions;

    internal CurrentGroupFlow(ICommandManager manager)
    {
        mManager = manager;
        mFixedOptions = new Dictionary<string, Guid>()
        {
            { "ManageCurrentGroup", Guid.NewGuid() },
            { "Back", Guid.NewGuid() },
            { "Home", Guid.NewGuid() },
            { "Exit", Guid.NewGuid() }
        };
    }

    void IFlow.Prompt()
    {
        string prompt = "Choose a group or a command:";

        List<(string, Guid)> options = mManager.GetItems()
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
            .ToList();

        if (FlowManager.IsManagementMode)
        {
            prompt = "Choose a group, a command, or an option:";
            options.Add(("Manage current group", mFixedOptions["ManageCurrentGroup"]));
        }

        Guid result = Prompter.Prompt(
            new SelectionPlugin<Guid>()
                .SetPrompt(prompt)
                .AddFlowDescriptions(mManager)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options.ToArray())
                .AddFlowCommonSelectionOptions(mFixedOptions["Back"], mFixedOptions["Home"], mFixedOptions["Exit"])
        );

        bool bCommonSelection = FlowManager.TryHandleCommonSelectionResult(
            result,
            (mFixedOptions["Back"], () => mManager.PrevGroup()),
            (mFixedOptions["Home"], () => mManager.GoMainGroup()),
            (mFixedOptions["Exit"], null)
        );

        if (bCommonSelection)
        {
            return;
        }

        switch (result)
        {
            case Guid id when id == mFixedOptions["ManageCurrentGroup"]:
                FlowManager.RunFlow("Command.CurrentGroup.Manage", mManager);
                break;
            default:
                if (mManager.IsCommand(result))
                {
                    if (FlowManager.IsManagementMode)
                    {
                        FlowManager.RunFlow("Command.CurrentCommand.Manage", mManager, result);
                    }
                    else
                    {
                        // FIX ME: Run command
                    }
                }
                else
                {
                    mManager.UseItem(result);
                    FlowManager.RunFlow("Command.CurrentGroup", mManager);
                }

                break;
        }
    }

    bool IFlow.TryJump(string command, bool bHasNext)
    {
        List<GroupItemModel> items = mManager.GetItems();

        foreach (GroupItemModel item in items)
        {
            if (item.Item is CommandModel commandModel && (commandModel.Name == command || commandModel.ShortName == command))
            {
                if (!bHasNext)
                {
                    // FIX ME: Run command
                }

                return !bHasNext;
            }
            else if (item.Item is GroupModel groupModel && (groupModel.Name == command || groupModel.ShortName == command))
            {
                mManager.UseItem(item.Id);
                FlowManager.JumpFlow("Command.CurrentGroup", mManager);

                return bHasNext;
            }
            else
            {
                continue;
            }
        }

        return false;
    }
}
