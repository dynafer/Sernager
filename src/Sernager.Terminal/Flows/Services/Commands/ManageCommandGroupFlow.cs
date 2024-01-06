using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Flows.Services.Commands;

internal static class ManageCommandGroupFlow
{
    internal const string NAME = "ManageCommandGroup";

    internal static void Run(ICommandManager manager)
    {
        Dictionary<string, Guid> fixedOptions = new Dictionary<string, Guid>
        {
            { ManageCurrentCommandGroupFlow.NAME, Guid.NewGuid() },
            { "Back", Guid.NewGuid() },
            { "Exit", Guid.NewGuid() }
        };

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            (string, Guid)[] options = manager.GetItems()
                .Select(x =>
                {
                    string type = x.Item switch
                    {
                        CommandModel => "Command",
                        GroupModel => "Group",
                        _ => "Unknown"
                    };

                    string name = x.Item switch
                    {
                        CommandModel command => command.Name,
                        GroupModel group => group.Name,
                        _ => "Unknown"
                    };

                    name += $" ({type})";

                    return (name, x.Id);
                })
                .ToArray();

            IBasePlugin plugin = new SelectionPlugin<Guid>()
                .SetPrompt("Choose a group, a command, or an option:")
                .AddDescription(CommandGroupFlowHelper.CreatePromptDescriptions(manager))
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options)
                .AddOptions(
                    ("Manage current group", fixedOptions[ManageCurrentCommandGroupFlow.NAME])
                )
                .AddFlowCommonOptions(fixedOptions["Back"], fixedOptions["Exit"]);

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            switch (result)
            {
                case Guid id when id == fixedOptions[ManageCurrentCommandGroupFlow.NAME]:
                    ManageCurrentCommandGroupFlow.Run(manager);
                    break;
                case Guid id when id == fixedOptions["Back"]:
                    manager.PrevGroup();
                    HistoryManager.Prev();
                    break;
                case Guid id when id == fixedOptions["Exit"]:
                    Environment.Exit(0);
                    break;
                default:
                    if (manager.IsCommand((Guid)result))
                    {
                        ManageCurrentCommandFlow.Run(manager, (Guid)result);
                    }
                    else
                    {
                        Run(manager.UseItem((Guid)result));
                    }

                    break;
            }
        };

        FlowManager.RunFlow(manager.CreateCommandGroupFlowName(NAME), pluginHandler, resultHandler);
    }
}
