using Sernager.Core.Extensions;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class EditNameFlow : IFlow
{
    private readonly IEnvironmentManager mManager;

    internal EditNameFlow(IEnvironmentManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Common", "EnterNamePromptWithCancelForEdit"))
                .SetInitialInput(mManager.EnvironmentGroup.Name)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string name) => name == mManager.EnvironmentGroup.Name || string.IsNullOrWhiteSpace(name),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseEnvironmentGroupName,
                            FlowManager.GetResourceString("Common", "NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        if (!string.IsNullOrWhiteSpace(name) && name != mManager.EnvironmentGroup.Name)
        {
            mManager.ChangeGroupName(name);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
