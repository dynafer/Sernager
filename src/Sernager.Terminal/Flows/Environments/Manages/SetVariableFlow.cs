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
    private readonly IEnvironmentManager mManager;
    private readonly bool mbSubst;

    internal SetVariableFlow(IEnvironmentManager manager, bool bSubst)
    {
        mManager = manager;
        mbSubst = bSubst;
    }

    void IFlow.Prompt()
    {
        string key = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Environment"))
                .SetPrompt("EnterKeyPromptWithCancel")
        );

        if (string.IsNullOrWhiteSpace(key))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        string value = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Environment"))
                .SetPrompt("EnterValuePromptWithCancel")
        );

        if (string.IsNullOrWhiteSpace(value))
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (mbSubst)
        {
            mManager.SetSubstVariable(key, value);
        }
        else
        {
            mManager.SetVariable(key, value);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
