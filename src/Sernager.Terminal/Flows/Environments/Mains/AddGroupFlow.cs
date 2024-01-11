using Sernager.Core.Helpers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Environments.Mains;

[Flow(Alias = "Environment.Main")]
internal sealed class AddGroupFlow : IFlow
{
    void IFlow.Prompt()
    {
        string groupName = Prompter.Prompt(
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
                            ManagerHelper.CanUseEnvironmentGroupName,
                            FlowManager.CommonResourcePack.GetString("NameExisted"),
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

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
