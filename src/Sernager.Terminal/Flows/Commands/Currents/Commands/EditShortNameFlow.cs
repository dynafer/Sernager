using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Currents.Commands;

[Flow(Alias = "Command.CurrentCommand.Manage")]
internal sealed class EditShortNameFlow : IFlow
{
    private readonly ICommandManager mManager;
    private readonly CommandModel mCommandModel;

    internal EditShortNameFlow(ICommandManager manager, CommandModel commandModel)
    {
        mManager = manager;
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        string shortName = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterShortNamePromptWithCancel")
                .SetInitialInput(mCommandModel.ShortName)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string shortNameInput) => shortNameInput == mCommandModel.ShortName,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => mManager.CanUseName(shortNameInput, true),
                            "ShortNameExisted",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        if (shortName != mCommandModel.ShortName)
        {
            mCommandModel.Name = shortName;
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
