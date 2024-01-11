using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage")]
internal sealed class AddCommandFlow : IFlow
{
    private readonly ICommandManager mManager;

    internal AddCommandFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Common", "EnterNamePromptWithCancelForAdd"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => mManager.CanUseName(name, false),
                            FlowManager.GetResourceString("Common", "NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Command", "EnterShortNamePromptWithSkip"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => mManager.CanUseName(shortNameInput, false),
                            FlowManager.GetResourceString("Command", "ShortNameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt(FlowManager.GetResourceString("Command", "EnterDescriptionPromptWithSkip"))
        );

        (string, string)[] environmentGroupOptions = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        string[] environmentGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .SetPrompt(FlowManager.GetResourceString("Command", "SelectEnvironmentGroupPromptWithSkip"))
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(environmentGroupOptions)
        ).ToArray();

        List<string> command = Prompter.Prompt(
            new EditorPlugin()
                .SetPrompt(FlowManager.GetResourceString("Command", "AddCommandPromptWithSkip"))
        ).ToList();

        bool bCommandArray = false;
        if (command.Count > 0)
        {
            bCommandArray = Prompter.Prompt(
                new ConfirmPlugin()
                    .SetPrompt(FlowManager.GetResourceString("Command", "AskSaveCommandAsArray"))
            );
        }

        CommandModel commandModel = new CommandModel()
        {
            Name = name,
            ShortName = shortName,
            Description = description,
            UsedEnvironmentGroups = environmentGroups.ToList()
        };

        if (bCommandArray)
        {
            commandModel.Command = command.ToArray();
        }
        else
        {
            commandModel.Command = string.Join(" ", command);
        }

        mManager.AddCommand(commandModel);

        FlowManager.RunPreviousFlow();
    }

    bool IFlow.TryJump(string _, bool __)
    {
        return false;
    }
}
