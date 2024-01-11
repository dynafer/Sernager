using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage")]
internal sealed class EditNameFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal EditNameFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Common", "EnterNamePromptWithCancelForEdit"))
                .SetInitialInput(mManager.CurrentGroup.Name)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string name) => name == mManager.CurrentGroup.Name || string.IsNullOrWhiteSpace(name),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => mManager.CanUseName(name, true),
                            FlowManager.GetResourceString("Common", "NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        if (name != mManager.CurrentGroup.Name && !string.IsNullOrWhiteSpace(name))
        {
            mManager.ChangeCurrentGroupName(name);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
