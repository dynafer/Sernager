using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents;

[Flow(Alias = "Command.CurrentCommand", Name = "Manage")]
internal sealed class ManageCommandFlow : IFlow
{
    private ICommandManager mManager;
    private Guid mCommandId;
    private CommandModel mCommandModel;

    internal ManageCommandFlow(ICommandManager manager, Guid commandId)
    {
        mManager = manager;
        mCommandId = commandId;
        mCommandModel = mManager.GetCommand(commandId);
    }

    void IFlow.Prompt()
    {
        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .AddFlowDescriptions(mManager, mCommandModel)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(
                    ("Edit command", "EditCommand"),
                    ("Edit name", "EditName"),
                    ("Edit short name", "EditShortName"),
                    ("Edit description", "EditDescription"),
                    ("Remove command", "RemoveCommand")
                )
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        switch (result)
        {
            case "EditCommand":
                FlowManager.RunFlow("Command.CurrentCommand.Manage.EditCommand", mCommandModel);
                break;
            case "EditName":
                FlowManager.RunFlow("Command.CurrentCommand.Manage.EditName", mManager, mCommandModel);
                break;
            case "EditShortName":
                FlowManager.RunFlow("Command.CurrentCommand.Manage.EditShortName", mManager, mCommandModel);
                break;
            case "EditDescription":
                FlowManager.RunFlow("Command.CurrentCommand.Manage.EditDescription", mCommandModel);
                break;
            case "RemoveCommand":
                mManager.RemoveItem(mCommandId);
                FlowManager.RunPreviousFlow();
                break;
            default:
                FlowManager.RunPreviousFlow();
                break;
        }
    }
}