using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage")]
internal sealed class EditDescriptionFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal EditDescriptionFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string description = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterDescriptionPrompt")
                .SetInitialInput(mManager.CurrentGroup.Description)
        );

        mManager.CurrentGroup.Description = description;

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
