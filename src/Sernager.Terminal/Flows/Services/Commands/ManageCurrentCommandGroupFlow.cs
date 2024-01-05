using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Flows.Helpers;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Services.Commands;

internal static class ManageCurrentCommandGroupFlow
{
    internal const string NAME = "ManageCurrentCommandGroup";

    internal static void Run(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            IBasePlugin plugin = new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .AddDescription(CommandGroupFlowHelper.CreatePromptDescriptions(manager))
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(
                    ("Edit name", "EditName"),
                    ("Edit short name", "EditShortName"),
                    ("Edit description", "EditDescription"),
                    ("Add a command", "AddCommand"),
                    ("Add a subgroup", "AddSubgroup"),
                    ("Remove a item(s)", "RemoveItems")
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
                        EditName(manager);
                        break;
                    case "EditShortName":
                        EditShortName(manager);
                        break;
                    case "EditDescription":
                        EditDescription(manager);
                        break;
                    case "AddCommand":
                        AddCommand(manager);
                        break;
                    case "AddSubgroup":
                        AddSubgroup(manager);
                        break;
                    case "RemoveItems":
                        RemoveItems(manager);
                        break;
                    default:
                        HistoryManager.Prev();
                        break;
                }
            }
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME), pluginHandler, resultHandler);
    }

    internal static void EditName(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return new InputPlugin()
                .SetPrompt("Enter a command group name without white spaces (Cancel: Empty or same input)")
                .SetInitialInput(manager.CurrentGroup.Name)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string name) => name == manager.CurrentGroup.Name || string.IsNullOrWhiteSpace(name),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => manager.CanUseName(name, true),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                );
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string groupName = ((string)result).Replace(" ", string.Empty);

            if (groupName == manager.CurrentGroup.Name || string.IsNullOrWhiteSpace(groupName))
            {
                HistoryManager.Prev();
                return;
            }

            manager.ChangeCurrentGroupName(groupName);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "EditName"), pluginHandler, resultHandler);
    }

    internal static void EditShortName(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return CommandGroupFlowHelper.CreateShortNamePromptPlugin(manager, true);
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string shortName = (string)result;

            bool bSkip = string.IsNullOrWhiteSpace(manager.CurrentGroup.ShortName)
                ? string.IsNullOrWhiteSpace(shortName)
                : manager.CurrentGroup.ShortName == shortName;

            if (bSkip)
            {
                HistoryManager.Prev();
                return;
            }

            manager.ChangeCurrentGroupShortName(shortName);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "EditShortName"), pluginHandler, resultHandler);
    }

    internal static void EditDescription(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return ManageFlowHelper.CreateDescriptionPromptPlugin(manager.CurrentGroup.Description, false);
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string description = (string)result;

            if (string.IsNullOrWhiteSpace(description))
            {
                HistoryManager.Prev();
                return;
            }

            manager.ChangeCurrentGroupDescription(description);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "EditDescription"), pluginHandler, resultHandler);
    }

    internal static void AddCommand(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return new InputPlugin()
                .SetPrompt("Enter a command name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => manager.CanUseName(name, false),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                );
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string groupName = ((string)result).Replace(" ", string.Empty);

            if (groupName == manager.CurrentGroup.Name || string.IsNullOrWhiteSpace(groupName))
            {
                HistoryManager.Prev();
                return;
            }

            string shortName = Prompter.Prompt(CommandFlowHelper.CreateShortNamePromptPlugin(manager));
            string description = Prompter.Prompt(ManageFlowHelper.CreateDescriptionPromptPlugin(string.Empty, true));
            string[] environmentGroups = Prompter.Prompt(CommandFlowHelper.CreateEnvironmentListPromptPlugin()).ToArray();
            string command = Prompter.Prompt(CommandFlowHelper.CreateCommandPromptPlugin());
            bool bCommandArray = false;

            if (!string.IsNullOrWhiteSpace(command))
            {
                bCommandArray = Prompter.Prompt(CommandFlowHelper.CreateCommandArrayPromptPlugin());
            }

            CommandModel commandModel = new CommandModel()
            {
                Name = groupName,
                ShortName = shortName,
                Description = description,
                UsedEnvironmentGroups = environmentGroups.ToList(),
                Command = bCommandArray
                    ? CommandFlowHelper.ToCommandArray(command)
                    : command
            };

            manager.AddCommand(commandModel);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "AddCommand"), pluginHandler, resultHandler);
    }

    internal static void AddSubgroup(ICommandManager manager)
    {
        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return new InputPlugin()
                .SetPrompt("Enter a command group name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => manager.CanUseName(name, false),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                );
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string groupName = ((string)result).Replace(" ", string.Empty);

            if (string.IsNullOrWhiteSpace(groupName))
            {
                HistoryManager.Prev();
                return;
            }

            string shortName = Prompter.Prompt(CommandGroupFlowHelper.CreateShortNamePromptPlugin(manager, false));
            string description = Prompter.Prompt(ManageFlowHelper.CreateDescriptionPromptPlugin(string.Empty, true));

            manager.AddSubgroup(groupName, shortName, description);

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "AddSubgroup"), pluginHandler, resultHandler);
    }

    internal static void RemoveItems(ICommandManager manager)
    {
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

            IBasePlugin plugin = new MultiSelectionPlugin<Guid>()
                .SetPrompt("Select a group or a command to remove (Cancel: No selection):")
                .AddDescription(CommandGroupFlowHelper.CreatePromptDescriptions(manager))
                .SetPageSize(5)
                .UseAutoComplete()
                .AddOptions(options);

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            IEnumerable<Guid> selectedItems = (IEnumerable<Guid>)result;

            foreach (Guid id in selectedItems)
            {
                manager.RemoveItem(id);
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, "RemoveItems"), pluginHandler, resultHandler);
    }
}
