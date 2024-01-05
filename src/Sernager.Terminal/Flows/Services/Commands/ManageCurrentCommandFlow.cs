using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Services.Commands;

internal static class ManageCurrentCommandFlow
{
    internal const string NAME = "ManageCurrentCommand";

    internal static void Run(ICommandManager manager, Guid commandId)
    {
        CommandModel command = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            IBasePlugin plugin = new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .AddDescription(CommandFlowHelper.CreatePromptDescriptions(manager, command))
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(
                    ("Edit name", "EditName"),
                    ("Edit short name", "EditShortName"),
                    ("Edit description", "EditDescription"),
                    ("Edit command", "EditCommand"),
                    ("Remove command", "RemoveCommand")
                )
                .AddFlowCommonOptions();

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            if (!FlowManager.TryHandleCommonOptions((string)result))
            {
                switch (result)
                {
                    case "EditName":
                        break;
                    case "EditShortName":
                        break;
                    case "EditDescription":
                        break;
                    case "EditCommand":
                        break;
                    case "RemoveCommand":
                        break;
                    default:
                        break;
                }
            }
        };

        FlowManager.RunFlow(NAME, pluginHandler, resultHandler);
    }
}
