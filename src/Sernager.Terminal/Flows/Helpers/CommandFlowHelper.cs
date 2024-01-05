using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

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
}
