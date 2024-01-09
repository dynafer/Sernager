using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class SetVariableFlow : IFlow
{
    private IEnvironmentManager mManager;
    private bool mbPre;

    internal SetVariableFlow(IEnvironmentManager manager, bool bPre)
    {
        mManager = manager;
        mbPre = bPre;
    }

    void IFlow.Prompt()
    {
        string key = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a key (Cancel: Empty input)")
        );

        if (string.IsNullOrWhiteSpace(key))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        string value = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a value (Cancel: Empty input)")
        );

        if (string.IsNullOrWhiteSpace(value))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (mbPre)
        {
            mManager.SetPreVariable(key, value);
        }
        else
        {
            mManager.SetVariable(key, value);
        }

        FlowManager.RunPreviousFlow();
    }
}
