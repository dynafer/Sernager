using Sernager.Core.Managers;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;

namespace Sernager.Terminal.Flows.Environments.Manages;

[Flow(Alias = "Environment.Manage")]
internal sealed class AddFromFileFlow : IFlow
{
    private readonly IEnvironmentManager mManager;
    private readonly bool mbPre;

    internal AddFromFileFlow(IEnvironmentManager manager, bool bPre)
    {
        mManager = manager;
        mbPre = bPre;
    }

    void IFlow.Prompt()
    {
        string prompt = FlowManager.GetResourceString("Environment", mbPre ? "SelectPreEnvFilePrompt" : "SelectEnvFilePrompt");
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
