using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Resources;
using Sernager.Terminal.Attributes;
using Sernager.Terminal.Flows.Extensions;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins;
using System.Diagnostics;

namespace Sernager.Terminal.Flows.Commands;

[Flow(Alias = "Command")]
internal sealed class CurrentGroupFlow : IFlow
{
    private readonly ICommandManager mManager;
    private readonly Dictionary<string, Guid> mFixedOptions;

    internal CurrentGroupFlow(ICommandManager manager)
    {
        mManager = manager;
        mFixedOptions = new Dictionary<string, Guid>()
        {
            { "ManageCurrentGroup", Guid.NewGuid() },
            { "Back", Guid.NewGuid() },
            { "Home", Guid.NewGuid() },
            { "Exit", Guid.NewGuid() }
        };
    }

    void IFlow.Prompt()
    {

        (string, Guid)[] options = mManager.GetItems()
            .Select(item =>
            {
                string itemString = $"{item.GetNameString()} ({item.GetTypeString()})";

                return (itemString, item.Id);
            })
            .ToArray();

        List<(string, Guid)> managementOptions = new List<(string, Guid)>();

        if (FlowManager.IsManagementMode)
        {
            managementOptions.Add(("ManageCurrentGroup", mFixedOptions["ManageCurrentGroup"]));
        }

        Guid result = Prompter.Prompt(
            new SelectionPlugin<Guid>()
                .UseResourcePack(FlowManager.GetResourceNamespace("Command"))
                .SetPrompt(FlowManager.IsManagementMode ? "CurrentGroupManagePrompt" : "CurrentGroupRunPrompt")
                .AddFlowDescriptions(mManager)
                .SetPageSize(FlowManager.PageSize)
                .UseAutoComplete()
                .AddOptions(options.ToArray())
                .AddOptionsUsingResourcePack(managementOptions.ToArray())
                .AddFlowCommonSelectionOptions(mFixedOptions["Back"], mFixedOptions["Home"], mFixedOptions["Exit"])
        );

        bool bCommonSelection = FlowManager.TryHandleCommonSelectionResult(
            result,
            (mFixedOptions["Back"], () => mManager.PrevGroup()),
            (mFixedOptions["Home"], () => mManager.GoMainGroup()),
            (mFixedOptions["Exit"], null)
        );

        if (bCommonSelection)
        {
            return;
        }

        switch (result)
        {
            case Guid id when id == mFixedOptions["ManageCurrentGroup"]:
                FlowManager.RunFlow("Command.CurrentGroup.Manage", mManager);
                break;
            default:
                if (mManager.IsCommand(result))
                {
                    if (FlowManager.IsManagementMode)
                    {
                        FlowManager.RunFlow("Command.CurrentCommand.Manage", mManager, result);
                    }
                    else
                    {
                        runCommand(result);
                        FlowManager.RunLastFlow();
                    }
                }
                else
                {
                    mManager.UseItem(result);
                    FlowManager.RunFlow("Command.CurrentGroup", mManager);
                }

                break;
        }
    }

    bool IFlow.TryJump(string command, bool bHasNext)
    {
        List<GroupItemModel> items = mManager.GetItems();

        foreach (GroupItemModel item in items)
        {
            if (item.Item is CommandModel commandModel && (commandModel.Name == command || commandModel.ShortName == command))
            {
                if (FlowManager.IsManagementMode)
                {
                    FlowManager.JumpFlow("Command.CurrentCommand.Manage", mManager, item.Id);
                }
                else
                {
                    if (!bHasNext)
                    {
                        runCommand(item.Id);
                        FlowManager.RunLastFlow();
                    }
                }

                return !bHasNext;
            }
            else if (item.Item is GroupModel groupModel && (groupModel.Name == command || groupModel.ShortName == command))
            {
                mManager.UseItem(item.Id);
                FlowManager.JumpFlow("Command.CurrentGroup", mManager);

                return bHasNext;
            }
            else
            {
                continue;
            }
        }

        return false;
    }

    private void runCommand(Guid commandId)
    {
        IExecutor executor = Program.Service.GetExecutor(commandId);
        CommandModel model = mManager.GetCommand(commandId);
        IResourcePack resourcePack = ResourceRetriever.UsePack(FlowManager.GetResourceNamespace("Command"));

        if (executor == null)
        {
            return;
        }

        string command = model.ToCommandString();

        using (Renderer renderer = new Renderer(Prompter.Writer))
        {
            List<IPromptComponent> components = new List<IPromptComponent>(Console.WindowHeight - 1)
            {
                new TextComponent()
                    .SetText($"{resourcePack.GetString("RunningCommand")}: {command}")
                    .SetTextColor(EColorFlags.BrightBlack)
                    .UseLineBreak()
            };

            renderer.Render(components.ToList());

            DataReceivedEventHandler outputHandler = (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                if (components.Count >= Console.WindowHeight - 1)
                {
                    components.RemoveAt(1);
                }

                components.Add(new TextComponent()
                    .SetText(e.Data)
                    .UseLineBreak()
                );

                renderer.Render(components.ToList());
            };

            DataReceivedEventHandler errorHandler = (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                components.Add(new TextComponent()
                    .SetText(e.Data)
                    .SetTextColor(EColorFlags.Red)
                    .UseLineBreak()
                );

                renderer.Render(components.ToList());
            };

            executor.Execute(outputHandler, errorHandler);
        }
    }
}
