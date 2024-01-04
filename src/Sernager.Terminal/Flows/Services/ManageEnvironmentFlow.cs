using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Services;

internal static class ManageEnvironmentFlow
{
    internal static void Run()
    {
        IBasePlugin plugin = new SelectionPlugin<string>()
            .SetPrompt("Choose an option:")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(
                ("Parse an Environment file", "ParseEnvFile"),
                ("Manage groups", "ManageCommandGroups"),
                ("Save as ...", "SaveAs")
            )
            .AddFlowCommonOptions();
        // FIX ME: Implement this flow
    }
}
