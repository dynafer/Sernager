using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents;

[Flow(Alias = "Command.CurrentGroup", Name = "Manage")]
internal sealed class ManageGroupFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal ManageGroupFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt(FlowManager.GetResourceString("Common", "ChooseOptionPrompt"))
                .AddFlowDescriptions(mManager)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(
                    (FlowManager.GetResourceString("Command", "AddCommand"), "AddCommand"),
                    (FlowManager.GetResourceString("Command", "AddSubgroup"), "AddSubgroup"),
                    (FlowManager.GetResourceString("Common", "EditName"), "EditName"),
                    (FlowManager.GetResourceString("Command", "EditShortName"), "EditShortName"),
                    (FlowManager.GetResourceString("Command", "EditDescription"), "EditDescription"),
                    (FlowManager.GetResourceString("Command", "RemoveItem"), "RemoveItem"),
                    (FlowManager.GetResourceString("Common", "RemoveThisGroup"), "RemoveGroup")
                )
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        switch (result)
        {
            case "AddCommand":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.AddCommand", mManager);
                break;
            case "AddSubgroup":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.AddSubgroup", mManager);
                break;
            case "EditName":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.EditName", mManager);
                break;
            case "EditShortName":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.EditShortName", mManager);
                break;
            case "EditDescription":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.EditDescription", mManager);
                break;
            case "RemoveItem":
                FlowManager.RunFlow("Command.CurrentGroup.Manage.RemoveItem", mManager);
                break;
            case "RemoveGroup":
                mManager.RemoveCurrentGroup();
                FlowManager.RunPreviousFlow(2);
                break;
            default:
                break;
        }
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
