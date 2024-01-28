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
                .SetPrompt(FlowManager.CommonResourcePack.GetString("EnterNamePromptWithCancelForAdd"))
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string name) => mManager.CanUseName(name, true),
                            FlowManager.CommonResourcePack.GetString("NameExisted"),
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string shortName = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterShortNamePromptWithSkip")
                .UseValidator(new InputValidator()
                    .AddRules(
                        (
                            string.IsNullOrWhiteSpace,
                            null,
                            EInputValidatorHandlerType.ReturnWhenTrue
                        ),
                        (
                            (string shortNameInput) => mManager.CanUseName(shortNameInput, true),
                            "ShortNameExisted",
                            EInputValidatorHandlerType.Default
                        )
                    )
                )
        ).Replace(" ", string.Empty);

        string description = Prompter.Prompt(
            new InputPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("EnterDescriptionPromptWithSkip")
        );

        (string, string)[] environmentGroupOptions = Program.Service.GetEnvironmentGroupNames()
            .Select(x => (x, x))
            .ToArray();

        string[] environmentGroups = Prompter.Prompt(
            new MultiSelectionPlugin<string>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("SelectEnvironmentGroupPromptWithSkip")
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(environmentGroupOptions)
        ).ToArray();

        string[] command = Prompter.Prompt(
            new EditorPlugin()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt("AddCommandPromptWithSkip")
        ).ToArray();

        bool bCommandArray = false;
        if (command.Length > 1 || (command.Length == 1 && !string.IsNullOrWhiteSpace(command[0])))
        {
            bCommandArray = Prompter.Prompt(
                new ConfirmPlugin()
                    .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                    .SetPrompt("AskSaveCommandAsArray")
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
            commandModel.Command = command;
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
