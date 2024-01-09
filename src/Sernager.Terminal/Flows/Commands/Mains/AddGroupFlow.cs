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
                .SetPrompt("Enter a command group name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseCommandGroupName,
                            "The name already exists.",
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
                .SetPrompt("Enter a command group short name without white spaces (Skip: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseCommandGroupName,
                            "The short name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command group description (Skip: Empty input)")
        );

        Program.Service.ManageCommandGroup(groupName, shortName, description);
        FlowManager.RunPreviousFlow();
    }
}
