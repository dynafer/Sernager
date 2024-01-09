using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments;

[Flow(Alias = "Environment")]
internal sealed class ManageFlow : IFlow
{
    private IEnvironmentManager mManager;

    internal ManageFlow(IEnvironmentManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string result = Prompter.Prompt(
            new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .AddFlowDescriptions(mManager)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(
                    ("Change addition mode", "ChangeAdditionMode"),
                    ("Add a pre-env file", "AddPreFile"),
                    ("Add an env file", "AddFile"),
                    ("Set a pre-environment variable", "SetPreVariable"),
                    ("Set an environment variable", "SetVariable"),
                    ("Edit pre-environment variables", "EditPreEnvironmentVaraibles"),
                    ("Edit environment variables", "EditEnvironmentVaraibles"),
                    ("Edit name", "EditName"),
                    ("Remove this group", "RemoveGroup")
                )
                .AddFlowCommonSelectionOptions()
        );

        if (FlowManager.TryHandleCommonSelectionResult(result))
        {
            return;
        }

        switch (result)
        {
            case "ChangeAdditionMode":
                FlowManager.RunFlow("Environment.Manage.ChangeAdditionMode", mManager);
                break;
            case "AddPreFile":
                break;
            case "AddFile":
                break;
            case "SetPreVariable":
                break;
            case "SetVariable":
                break;
            case "EditPreEnvironmentVaraibles":
                break;
            case "EditEnvironmentVaraibles":
                break;
            case "EditName":
                FlowManager.RunFlow("Environment.Manage.EditName", mManager);
                break;
            case "RemoveGroup":
                mManager.RemoveGroup();
                FlowManager.RunPreviousFlow();
                break;
            default:
                FlowManager.RunPreviousFlow();
                break;
        }
    }
}
