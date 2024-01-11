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
internal sealed class AddSubgroupFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal AddSubgroupFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.CommonResourcePack.GetString("EnterNamePromptWithCancelForAdd"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string nameInput) => mManager.CanUseName(nameInput, false),
                            FlowManager.CommonResourcePack.GetString("NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        if (string.IsNullOrWhiteSpace(name))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterShortNamePromptWithSkip")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string nameInput) => mManager.CanUseName(nameInput, false),
                            "ShortNameExisted",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        string description = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterDescriptionPromptWithSkip")
        );

        mManager.AddSubgroup(name, shortName, description);

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
