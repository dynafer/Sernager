using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class EditEnvironmentVaraibleFlow : IFlow
{
    private readonly IEnvironmentManager mManager;
    private readonly bool mbSubst;

    internal EditEnvironmentVaraibleFlow(IEnvironmentManager manager, bool bSubst)
    {
        mManager = manager;
        mbSubst = bSubst;
    }

    void IFlow.Prompt()
    {
        Dictionary<string, string> variables = mbSubst ? mManager.Group.SubstVariables : mManager.Group.Variables;
        string[] initialLines = variables.Select(x => $"{x.Key}={x.Value}").ToArray();
        string prompt = mbSubst ? "EditSubstEnvironmentVariablePrompt" : "EditEnvironmentVariablePrompt";

        string[] editedVariables = Prompter.Prompt(
            new EditorPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Environment"))
                .SetPrompt(prompt)
                .SetInitialLines(initialLines)
        ).ToArray();

        if (editedVariables == null)
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (mbSubst)
        {
            mManager.RemoveSubstVariables(variables.Keys.ToArray());
            mManager.AddSubstLines(editedVariables);
        }
        else
        {
            mManager.RemoveVariables(variables.Keys.ToArray());
            mManager.AddLines(editedVariables);
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
