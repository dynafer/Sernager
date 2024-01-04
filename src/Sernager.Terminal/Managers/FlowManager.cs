using Sernager.Core;
using Sernager.Terminal.Flows;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Managers;

internal static class FlowManager
{
    private static readonly Dictionary<string, Guid> mHistoryIds = new Dictionary<string, Guid>();

    internal static void Start(string[] commands)
    {
        Home(commands.Length > 0);
    }

    internal static void Home(bool bSkip = false)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(
                    ("Run command", RunCommandFlow.NAME),
                    ("Manage services", ManageServiceFlow.NAME),
                    ("Exit", "Exit")
                );
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            switch (result)
            {
                case RunCommandFlow.NAME:
                    RunCommandFlow.Run();
                    break;
                case ManageServiceFlow.NAME:
                    ManageServiceFlow.Run();
                    break;
                default:
                    Environment.Exit(0);
                    break;
            }
        };

        RunFlow("Home", pluginHandler, resultHandler, bSkip);
    }

    internal static void RunFlow(string key, HistoryPromptPluginHandler pluginHandler, HistoryResultHandler resultHandler, bool bSkip = false)
    {
        if (!mHistoryIds.ContainsKey(key))
        {
            HistoryModel model = new HistoryModel(pluginHandler, resultHandler);

            mHistoryIds.Add(key, model.Id);

            HistoryManager.Run(model, bSkip);
        }
        else
        {
            HistoryManager.Run(mHistoryIds[key], bSkip);
        }
    }

    internal static TPlugin AddFlowCommonOptions<TPlugin>(this TPlugin plugin)
        where TPlugin : ListBasePlugin<string>
    {
        EOptionTypeFlags optionType = plugin.ToOptionType();

        plugin.Options.Add(new OptionItem<string>(optionType, "Back", "Back"));
        plugin.Options.Add(new OptionItem<string>(optionType, "Exit", "Exit"));

        return plugin;
    }

    internal static TPlugin AddFlowCommonOptions<TPlugin, TOptionValue>(this TPlugin plugin, TOptionValue backOption, TOptionValue exitOption)
        where TPlugin : ListBasePlugin<TOptionValue>
        where TOptionValue : notnull
    {
        EOptionTypeFlags optionType = plugin.ToOptionType();

        plugin.Options.Add(new OptionItem<TOptionValue>(optionType, "Back", backOption));
        plugin.Options.Add(new OptionItem<TOptionValue>(optionType, "Exit", exitOption));

        return plugin;
    }

    internal static bool TryHandleCommonOptions(string result)
    {
        switch (result)
        {
            case "Back":
                HistoryManager.Prev();
                return true;
            case "Exit":
                Environment.Exit(0);
                return true;
            default:
                return false;
        }
    }

    internal static bool TryHandleCommonOptions<TOptionValue>(TOptionValue result, TOptionValue backOption, TOptionValue exitOption)
        where TOptionValue : notnull
    {
        if (EqualityComparer<TOptionValue>.Default.Equals(result, backOption))
        {
            HistoryManager.Prev();
            return true;
        }
        else if (EqualityComparer<TOptionValue>.Default.Equals(result, exitOption))
        {
            Environment.Exit(0);
            return true;
        }
        else
        {
            return false;
        }
    }
}
