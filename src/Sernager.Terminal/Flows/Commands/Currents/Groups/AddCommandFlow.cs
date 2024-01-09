using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Plugins;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Flows.Commands.Currents.Groups;

[Flow(Alias = "Command.CurrentGroup.Manage")]
internal sealed class AddCommandFlow : IFlow
{
    private ICommandManager mManager;

    internal AddCommandFlow(ICommandManager manager)
    {
        mManager = manager;
    }

    void IFlow.Prompt()
    {
        string name = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => mManager.CanUseName(name, false),
                            "The name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command short name without white spaces (Cancel: Empty input)")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => mManager.CanUseName(shortNameInput, false),
                            "The short name already exists.",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string description = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command description")
        );

        (string, string)[] environmentGroupOptions = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        string[] environmentGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .SetPrompt("Select an environment group(s) (Skip: No selection)")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(environmentGroupOptions)
        ).ToArray();

        string command = Prompter.Prompt(
            new InputPlugin()
                .SetPrompt("Enter a command (Skip: Empty input)")
        );

        bool bCommandArray = false;
        if (!string.IsNullOrWhiteSpace(command))
        {
            bCommandArray = Prompter.Prompt(
                new ConfirmPlugin()
                    .SetPrompt("Do you want to save the command as an array?")
            );
        }

        CommandModel commandModel = new CommandModel()
        {
            Name = name,
            ShortName = shortName,
            Description = description,
            UsedEnvironmentGroups = environmentGroups.ToList(),
            Command = command
        };

        if (bCommandArray)
        {
            commandModel.ToCommandAsArray();
        }

        mManager.AddCommand(commandModel);

        FlowManager.RunPreviousFlow();
    }
}
