using Sernager.Core.Helpers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Mains;

[Flow(Alias = "Command.Main")]
internal sealed class AddGroupFlow : IFlow
{
    void IFlow.Prompt()
    {
        string groupName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Common", "EnterNamePromptWithCancelForAdd"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseCommandGroupName,
                            FlowManager.GetResourceString("Common", "NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        if (string.IsNullOrWhiteSpace(groupName))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Command", "EnterShortNamePromptWithSkip"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseCommandGroupName,
                            FlowManager.GetResourceString("Command", "ShortNameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Command", "EnterDescriptionPromptWithSkip"))
        );

        Program.Service.ManageCommandGroup(groupName, shortName, description);
        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
