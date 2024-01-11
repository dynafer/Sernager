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
    private readonly ICommandManager mManager;
    private readonly Guid mCommandId;
    private readonly CommandModel mCommandModel;

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
                .SetPrompt(FlowManager.GetResourceString("Common", "ChooseOptionPrompt"))
                .AddFlowDescriptions(mManager, mCommandModel)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(
                    (FlowManager.GetResourceString("Command", "EditCommand"), "EditCommand"),
                    (FlowManager.GetResourceString("Common", "EditName"), "EditName"),
                    (FlowManager.GetResourceString("Command", "EditShortName"), "EditShortName"),
                    (FlowManager.GetResourceString("Command", "EditDescription"), "EditDescription"),
                    (FlowManager.GetResourceString("Command", "RemoveCommand"), "RemoveCommand")
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

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
