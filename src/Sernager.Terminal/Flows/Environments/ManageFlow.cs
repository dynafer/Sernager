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
                    ("Add from a pre-env file", "AddFromPreFile"),
                    ("Add from an env file", "AddFromFile"),
                    ("Set a pre-environment variable", "SetPreVariable"),
                    ("Set an environment variable", "SetVariable"),
                    ("Edit pre-environment variables", "EditPreEnvironmentVaraible"),
                    ("Edit environment variables", "EditEnvironmentVaraible"),
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
            case "AddFromPreFile":
                FlowManager.RunFlow("Environment.Manage.AddFromFile", mManager, true);
                break;
            case "AddFromFile":
                FlowManager.RunFlow("Environment.Manage.AddFromFile", mManager, false);
                break;
            case "SetPreVariable":
                FlowManager.RunFlow("Environment.Manage.SetVariable", mManager, true);
                break;
            case "SetVariable":
                FlowManager.RunFlow("Environment.Manage.SetVariable", mManager, false);
                break;
            case "EditPreEnvironmentVaraible":
                FlowManager.RunFlow("Environment.Manage.EditEnvironmentVaraible", mManager, true);
                break;
            case "EditEnvironmentVaraible":
                FlowManager.RunFlow("Environment.Manage.EditEnvironmentVaraible", mManager, false);
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
