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
internal sealed class EditShortNameFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal EditShortNameFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string shortName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command group short name (Cancel: Same input)")
                .SetInitialInput(mManager.CurrentGroup.ShortName)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string shortNameInput) => shortNameInput == mManager.CurrentGroup.ShortName,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => mManager.CanUseName(shortNameInput, true),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        bool bSkip = string.IsNullOrWhiteSpace(mManager.CurrentGroup.ShortName)
            ? string.IsNullOrWhiteSpace(shortName)
            : mManager.CurrentGroup.ShortName == shortName;

        if (!bSkip)
        {
            mManager.ChangeCurrentGroupShortName(shortName);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
