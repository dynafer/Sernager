using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using System.Collections.ObjectModel;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class EditEnvironmentVaraibleFlow : IFlow
{
    private readonly IEnvironmentManager mManager;
    private readonly bool mbPre;

    internal EditEnvironmentVaraibleFlow(IEnvironmentManager manager, bool bPre)
    {
        mManager = manager;
        mbPre = bPre;
    }

    void IFlow.Prompt()
    {
        ReadOnlyDictionary<string, string> variables = mbPre ? mManager.GetPreVariables() : mManager.GetVariables();
        string[] initialLines = variables.Select(x => $"{x.Key}={x.Value}").ToArray();

        IEnumerable<string> editedVariables = Prompter.Prompt(
            new EditorPlugin()
                .SetPrompt($"Edit {(mbPre ? "pre-" : "")}environment variables")
                .SetInitialLines(initialLines)
        );

        if (editedVariables == null)
        {
            FlowManager.RunPreviousFlow();
            return;
        }

        if (mbPre)
        {
            mManager.RemovePreVariables(variables.Keys.ToArray());
            mManager.AddPreLines(editedVariables.ToArray());
        }
        else
        {
            mManager.RemoveVariables(variables.Keys.ToArray());
            mManager.AddLines(editedVariables.ToArray());
        }

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
