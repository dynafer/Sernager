using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class AddFromFileFlow : IFlow
{
    private readonly IEnvironmentManager mManager;
    private readonly bool mbSubst;

    internal AddFromFileFlow(IEnvironmentManager manager, bool bSubst)
    {
        mManager = manager;
        mbSubst = bSubst;
    }

    void IFlow.Prompt()
    {
        string prompt = mbSubst ? "SelectSubstEnvFilePrompt" : "SelectEnvFilePrompt";
        string extension = ".env";
        string? path;

        if (FlowPromptHelper.TrySelectFile("Environment", prompt, extension, FlowManager.PageSize, out path))
        {
            if (mbSubst)
            {
                mManager.AddFromSubstFile(path);
            }
            else
            {
                mManager.AddFromFile(path);
            }

            FlowManager.RunPreviousFlow();
        }
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
