using Sernager.Core.Extensions;
using Sernager.Core.Helpers;
using Sernager.Core.Managers;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Helpers;

internal static class CommandGroupFlowHelper
{
    internal static ITypePlugin<string> CreateShortNamePromptPlugin(string shortName, bool bSkip)
    {
        string promptSkipText = $"({(bSkip ? "Skip" : "Cancel")}: {(string.IsNullOrWhiteSpace(shortName) ? "Empty" : "Same")} input)";

        return new InputPlugin()
            .SetPrompt($"Enter a short name for the command group {promptSkipText}")
            .SetInitialInput(shortName)
            .UseValidator(new InputValidator()
                .AddRules(
                    (
                        (string shortNameInput) => string.IsNullOrWhiteSpace(shortName)
                            ? string.IsNullOrWhiteSpace(shortNameInput)
                            : shortNameInput == shortName,
                        null,
                        EInputValidatorHandlerType.ReturnWhenTrue
                    ),
                    (
                        ManagerHelper.CanUseCommandGroupName,
                        "The name already exists.",
                        EInputValidatorHandlerType.Default
                    )
                )
            );
    }

    internal static ITypePlugin<string> CreateShortNamePromptPlugin(ICommandManager manager, bool bEdit)
    {
        string promptSkipText = $"({(!bEdit ? "Skip" : "Cancel")}: {(!bEdit ? "Empty" : "Same")} input)";

        return new InputPlugin()
            .SetPrompt($"Enter a short name for the command group {promptSkipText}")
            .SetInitialInput(!bEdit ? string.Empty : manager.CurrentGroup.ShortName)
            .UseValidator(new InputValidator()
                .AddRules(
                    (
                        (string shortNameInput) => !bEdit
                            ? string.IsNullOrWhiteSpace(shortNameInput)
                            : shortNameInput == manager.CurrentGroup.ShortName,
                        null,
                        EInputValidatorHandlerType.ReturnWhenTrue
                    ),
                    (
                        (string shortNameInput) => manager.CanUseName(shortNameInput, bEdit),
                        "The name already exists.",
                        EInputValidatorHandlerType.Default
                    )
                )
            );
    }

    internal static string[] CreatePromptDescriptions(ICommandManager manager)
    {
        List<string> description = new List<string>();

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.ShortName))
        {
            description.Add($"Short name: {manager.CurrentGroup.ShortName}");
        }

        if (!string.IsNullOrWhiteSpace(manager.CurrentGroup.Description))
        {
            description.Add($"Description: {manager.CurrentGroup.Description}");
        }

        description.Add($"Path: {manager.CreateCommandGroupPath(" > ")}");

        return description.ToArray();
    }
}
