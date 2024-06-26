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
internal sealed class EditNameFlow : IFlow
{
    private readonly ICommandManager mManager;
    private readonly CommandModel mCommandModel;

    internal EditNameFlow(ICommandManager manager, CommandModel commandModel)
    {
        mManager = manager;
        mCommandModel = commandModel;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.CommonResourcePack.GetString("EnterNamePromptWithCancelForEdit"))
                .SetInitialInput(mCommandModel.Name)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string name) => name == mCommandModel.Name || string.IsNullOrWhiteSpace(name),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string nameInput) => mManager.CanUseName(nameInput, true),
                            FlowManager.CommonResourcePack.GetString("NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        if (name != mCommandModel.Name && !string.IsNullOrWhiteSpace(name))
        {
            mCommandModel.Name = name;
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
