using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage", Name = "EditDescription")]
internal sealed class EditDescriptionFlow : IFlow
{
    private ICommandManager mManager;

    internal EditDescriptionFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command group description")
                .SetInitialInput(mManager.CurrentGroup.Description)
        );

        mManager.CurrentGroup.Description = description;

        FlowManager.RunPreviousFlow();
    }
}
