using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Components.Cursors;
using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Prompts.Plugins;

internal sealed class InputPlugin : ITypePlugin<string>
{
    private AutoComplete<string> mInput = new AutoComplete<string>();
    private bool mbUseAutoComplete = false;
    private Func<InputPlugin, string, bool>? mValidator = null;
    public List<string>? Hints { get; private set; } = null;
    public string Prompt { get; set; } = string.Empty;
    public bool ShouldShowHints { get; set; } = false;
    public bool ShouldContinueToNextLine => true;

    internal InputPlugin UseAutoComplete()
    {
        Hints = new List<string>();
        mbUseAutoComplete = true;

        return this;
    }

    internal InputPlugin UseValidator(Func<InputPlugin, string, bool> validator)
    {
        mValidator = validator;

        return this;
    }

    bool IBasePlugin.Input(ConsoleKeyInfo keyInfo, out object result)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                if (mValidator != null && !mValidator(this, mInput.Input))
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

        components.Add(new CursorComponent()
            .AddCursor(ECursorDirection.HorizontalAbsolute, startPosition + mInput.CursorPosition)
        );

        return components;
    }
}
