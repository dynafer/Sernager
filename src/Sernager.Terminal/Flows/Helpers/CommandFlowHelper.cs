using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;
using System.Text;

namespace Sernager.Terminal.Flows.Helpers;

internal static class CommandFlowHelper
{
    internal static ITypePlugin<string> CreateShortNamePromptPlugin(ICommandManager manager)
    {
        return new InputPlugin()
            .SetPrompt("Enter a short name for the command (Skip: Empty input)")
            .UseValidator(new InputValidator()
                .AddRules(
                    (
                        string.IsNullOrWhiteSpace,
                        null,
                        EInputValidatorHandlerType.ReturnWhenTrue
                    ),
                    (
                        (string shortNameInput) => manager.CanUseName(shortNameInput, false),
                        "The name already exists.",
                        EInputValidatorHandlerType.Default
                    )
                )
            );
    }

    internal static ITypePlugin<string> CreateShortNamePromptPlugin(ICommandManager manager, CommandModel model)
    {
        return new InputPlugin()
            .SetPrompt("Enter a short name for the command (Cancel: Same input)")
            .SetInitialInput(model.ShortName)
            .UseValidator(new InputValidator()
                .AddRules(
                    (
                        (string shortNameInput) => shortNameInput == model.ShortName,
                        null,
                        EInputValidatorHandlerType.ReturnWhenTrue
                    ),
                    (
                        (string shortNameInput) => manager.CanUseName(shortNameInput, true),
                        "The name already exists.",
                        EInputValidatorHandlerType.Default
                    )
                )
            );
    }

    internal static IEnumerableResultBasePlugin<string> CreateEnvironmentListPromptPlugin()
    {
        (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        return new MultiSelectionPlugin<string>()
            .SetPrompt("Select an environment group(s) (Skip: No selection)")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(options);
    }

    internal static IEnumerableResultBasePlugin<string> CreateEnvironmentListPromptPlugin(CommandModel commandModel)
    {
        (string, string)[] options = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        return new MultiSelectionPlugin<string>()
            .SetPrompt("Select an environment group(s) (Skip: No selection)")
            .SetPageSize(5)
            .UseAutoComplete()
            .AddOptions(options)
            .SetInitialSelections(commandModel.UsedEnvironmentGroups.ToArray());
    }

    internal static ITypePlugin<string> CreateCommandPromptPlugin()
    {
        return new InputPlugin()
            .SetPrompt("Enter a command (Skip: Empty input)");
    }

    internal static ITypePlugin<string> CreateCommandPromptPlugin(CommandModel commandModel)
    {
        string command = commandModel.Command switch
        {
            string[] commandArray => string.Join(" ", commandArray),
            string commandString => commandString,
            _ => throw new ArgumentException("Command must be a string or string[]")
        };

        return new InputPlugin()
            .SetPrompt("Enter a command (Cancel: Same input)")
            .SetInitialInput(command);
    }

    internal static ITypePlugin<bool> CreateCommandArrayPromptPlugin()
    {
        return new ConfirmPlugin()
            .SetPrompt("Do you want to save the command as an array?");
    }

    internal static string[] CreatePromptDescriptions(ICommandManager manager, CommandModel commandModel)
    {
        List<string> description = new List<string>();

        if (!string.IsNullOrWhiteSpace(commandModel.ShortName))
        {
            description.Add($"Short name: {commandModel.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(commandModel.Description))
        {
            description.Add($"Description: {commandModel.Description}");
        }

        description.Add($"Path: {manager.CreateCommandPath(" > ")}.{commandModel.Name}");

        return description.ToArray();
    }

    internal static string[] ToCommandArray(string command, string delimiter = "\n")
    {
        command = command.Replace(delimiter, " ").Trim();

        StringBuilder sb = new StringBuilder();
        bool bInsideSingleQuote = false;
        bool bInsideDoubleQuote = false;

        foreach (char c in command)
        {
            if (c == '\'')
            {
                bInsideSingleQuote = !bInsideSingleQuote;
            }
            else if (c == '"')
            {
                bInsideDoubleQuote = !bInsideDoubleQuote;
            }
            else if (c == ' ' && !bInsideSingleQuote && !bInsideDoubleQuote)
            {
                sb.Append('\n');
                continue;
            }

            sb.Append(c);
        }

        command = sb.ToString();

        return command.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();
    }
}
