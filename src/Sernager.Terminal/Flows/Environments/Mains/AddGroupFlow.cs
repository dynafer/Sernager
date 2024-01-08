using Sernager.Core.Helpers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Environments.Mains;

[Flow(Alias = "Environment.Main", Name = "AddGroup")]
internal sealed class AddGroupFlow : IFlow
{
    void IFlow.Prompt()
    {
        string groupName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter an environment group name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            ManagerHelper.CanUseEnvironmentGroupName,
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        );

        if (!string.IsNullOrWhiteSpace(groupName))
        {
            Program.Service.ManageEnvironmentGroup(groupName);
        }

        FlowManager.RunPreviousFlow();
    }
}
