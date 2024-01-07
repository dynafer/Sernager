using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage", Name = "EditName")]
internal sealed class EditNameFlow : IFlow
{
    private ICommandManager mManager;

    internal EditNameFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command group name without white spaces (Cancel: Empty or same input)")
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
                            "The name already exists.",
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
}
