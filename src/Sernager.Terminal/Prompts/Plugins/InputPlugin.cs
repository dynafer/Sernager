using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class InputPlugin : ITypePlugin<string>
{
    private readonly AutoComplete<string> mInput = new AutoComplete<string>();
    private bool mbUseAutoComplete = false;
    private InputValidator? mValidator = null;
    public List<string>? Hints { get; private set; } = null;
    public string Prompt { get; set; } = string.Empty;
    public List<string> Description { get; init; } = new List<string>();
    public bool ShouldShowHints { get; set; } = false;
    public bool ShouldShowCursor => true;

    internal InputPlugin SetInitialInput(string input)
    {
        mInput.SetInitialInput(input);

        return this;
    }

    internal InputPlugin UseAutoComplete()
    {
        Hints = new List<string>();
        mbUseAutoComplete = true;

        return this;
    }

    internal InputPlugin UseValidator(InputValidator validator)
    {
        mValidator = validator;

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        if (mValidator != null)
        {
            mValidator.ErrorMessage = null;
        }

        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                if (mValidator != null && !mValidator.Validate(mInput.Input))
                {
                    result = null!;

                    return false;
                }

                result = mInput.Input;

                return true;
            case ConsoleKey.Tab:
                if (mbUseAutoComplete && Hints != null && Hints.Count > 0)
                {
                    mInput.CompleteByTab(Hints);
                }

                break;
            default:
                mInput.InterceptInput(keyInfo);

                break;
        }

        result = null!;

        return false;
    }

    List<IPromptComponent> IBasePlugin.Render()
    {
        List<IPromptComponent> components = [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt),
        ];

        int startPosition = Prompt.Length + 1;

        if (ShouldShowHints && Hints != null && Hints.Count > 0)
        {
            string hintString = $" ({string.Join("|", Hints)})";

            components.Add(new TextComponent()
                .SetTextColor(EColorFlags.BrightBlack)
                .SetText(hintString));

            startPosition += hintString.Length;
        }

        components.Add(new TextComponent()
            .SetDecoration(EDecorationFlags.Bold)
            .SetText(": "));
        startPosition += 2;

        components.Add(new TextComponent()
            .SetText(mInput.Input));

        if (mbUseAutoComplete && Hints != null && Hints.Count > 0)
        {
            int suggestionIndex = mInput.GetFirstSuggestionIndex(Hints);
            if (suggestionIndex != -1)
            {
                components.Add(new TextComponent()
                    .SetTextColor(EColorFlags.BrightBlack)
                    .SetText(Hints[suggestionIndex].Substring(mInput.Input.Length))
                );
            }
        }

        if (Description.Count > 0)
        {
            components.Add(new LineBreakComponent());
            components.AddRange(
                Description
                    .Select((string description) =>
                    {
                        return new TextComponent()
                            .SetTextColor(EColorFlags.BrightBlack)
                            .SetText(description)
                            .UseLineBreak();
                    })
            );
            components.Add(new CursorComponent()
                .AddCursor(ECursorDirection.Up, Description.Count + 1)
            );
        }

        if (mValidator != null && mValidator.ErrorMessage != null)
        {
            components.AddRange([
                new LineBreakComponent(),
                new TextComponent()
                    .SetTextColor(EColorFlags.Red)
                    .SetText(mValidator.ErrorMessage),
                new CursorComponent()
                    .AddCursor(ECursorDirection.Up, Description.Count + 1)
            ]);
        }

        components.Add(new CursorComponent()
            .AddCursor(ECursorDirection.HorizontalAbsolute, startPosition + mInput.CursorPosition)
        );

        return components;
    }

    List<IPromptComponent> IBasePlugin.RenderLast()
    {
        List<IPromptComponent> components = [
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetText(Prompt),
            new CursorComponent()
                .AddCursor(ECursorDirection.Right, 1),
            new TextComponent()
                .SetDecoration(EDecorationFlags.Bold)
                .SetTextColor(EColorFlags.Green)
                .SetText(mInput.Input)
                .UseLineBreak(),
        ];

        return components;
    }
}
