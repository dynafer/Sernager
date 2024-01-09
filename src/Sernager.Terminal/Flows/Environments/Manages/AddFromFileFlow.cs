using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class AddFromFileFlow : IFlow
{
    private IEnvironmentManager mManager;
    private bool mbPre;

    internal AddFromFileFlow(IEnvironmentManager manager, bool bPre)
    {
        mManager = manager;
        mbPre = bPre;
    }

    void IFlow.Prompt()
    {
        string prompt = $"Select {(mbPre ? "a pre-env" : "an env")} file (.env extension only):";
        string extension = ".env";
        string path;

        if (FlowPromptHelper.TrySelectFile(prompt, extension, FlowManager.PageSize, out path))
        {
            if (mbPre)
            {
                mManager.AddFromPreFile(path);
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
