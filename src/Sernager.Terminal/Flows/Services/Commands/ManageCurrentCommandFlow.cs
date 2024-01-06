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

internal static class ManageCurrentCommandFlow
{
    internal const string NAME = "ManageCurrentCommand";

    internal static void Run(ICommandManager manager, Guid commandId)
    {
        CommandModel commandModel = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            IBasePlugin plugin = new SelectionPlugin<string>()
                .SetPrompt("Choose an option:")
                .AddDescription(CommandFlowHelper.CreatePromptDescriptions(manager, commandModel))
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
                        EditName(manager, commandId);
                        break;
                    case "EditShortName":
                        EditShortName(manager, commandId);
                        break;
                    case "EditDescription":
                        EditDescription(manager, commandId);
                        break;
                    case "EditCommand":
                        EditCommand(manager, commandId);
                        break;
                    case "RemoveCommand":
                        manager.RemoveItem(commandId);
                        HistoryManager.Prev();
                        break;
                    default:
                        HistoryManager.Prev();
                        break;
                }
            }
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, commandModel.Name), pluginHandler, resultHandler);
    }

    internal static void EditName(ICommandManager manager, Guid commandId)
    {
        CommandModel commandModel = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            IBasePlugin plugin = new InputPlugin()
                .SetPrompt("Enter a command name without white spaces (Cancel: Empty or same input)")
                .SetInitialInput(commandModel.Name)
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            (string name) => name == commandModel.Name || string.IsNullOrWhiteSpace(name),
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string nameInput) => manager.CanUseName(nameInput, true),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                );

            return plugin;
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string name = (string)result;
            if (name != commandModel.Name && !string.IsNullOrWhiteSpace(name))
            {
                commandModel.Name = name;
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, commandModel.Name, "EditName"), pluginHandler, resultHandler);
    }

    internal static void EditShortName(ICommandManager manager, Guid commandId)
    {
        CommandModel commandModel = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return CommandFlowHelper.CreateShortNamePromptPlugin(manager, commandModel);
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string shortName = (string)result;
            if (shortName != commandModel.ShortName)
            {
                commandModel.ShortName = shortName;
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, commandModel.Name, "EditShortName"), pluginHandler, resultHandler);
    }

    internal static void EditDescription(ICommandManager manager, Guid commandId)
    {
        CommandModel commandModel = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return ManageFlowHelper.CreateDescriptionPromptPlugin(commandModel.Description, false);
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            commandModel.Description = (string)result;

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, commandModel.Name, "EditDescription"), pluginHandler, resultHandler);
    }

    internal static void EditCommand(ICommandManager manager, Guid commandId)
    {
        CommandModel commandModel = manager.GetCommand(commandId);

        HistoryPromptPluginHandler pluginHandler = () =>
        {
            return CommandFlowHelper.CreateCommandPromptPlugin(commandModel);
        };

        HistoryResultHandler resultHandler = (object result) =>
        {
            string oldCommand = commandModel.Command switch
            {
                string[] commandArray => string.Join(" ", commandArray),
                string commandString => commandString,
                _ => throw new ArgumentException("Command must be a string or string[]")
            };

            string command = (string)result;

            if (command == oldCommand)
            {
                HistoryManager.Prev();
                return;
            }

            if (!string.IsNullOrWhiteSpace(command))
            {
                bool bCommandArray = Prompter.Prompt(CommandFlowHelper.CreateCommandArrayPromptPlugin());

                if (bCommandArray)
                {
                    commandModel.Command = CommandFlowHelper.ToCommandArray(command);
                }
                else
                {
                    commandModel.Command = command;
                }
            }
            else
            {
                commandModel.Command = command;
            }

            HistoryManager.Prev();
        };

        FlowManager.RunFlow(manager.CreateCommandFlowName(NAME, commandModel.Name, "EditCommand"), pluginHandler, resultHandler);
    }
}
