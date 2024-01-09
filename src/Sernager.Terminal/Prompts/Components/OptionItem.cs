using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class OptionItem<T>
    where T : notnull
{
    private readonly EOptionTypeFlags mType = EOptionTypeFlags.None;
    internal string Name { get; init; } = string.Empty;
    internal T Value { get; init; }
    internal bool IsSelected { get; private set; } = false;

    internal OptionItem(EOptionTypeFlags type, string name, T value)
    {
        mType = type;
        Name = name;
        Value = value;
    }

    internal void ToggleSelection()
    {
        IsSelected = !IsSelected;
    }

    internal TextComponent ToTextComponent(bool bCurrentOrEnds = false)
    {
        IPromptComponent name = new InlineStyledTextComponent()
            .SetText(Name);

        TextComponent component = new TextComponent()
            .SetDecoration(getDecoration(bCurrentOrEnds))
            .SetTextColor(getColor(bCurrentOrEnds))
            .SetText(getPrefix(bCurrentOrEnds) + name.Render())
            .UseLineBreak();

        return component;
    }

    internal TextComponent ToRestTextComponent()
    {
        IPromptComponent name = new InlineStyledTextComponent()
            .SetText(Name)
            .UseEscapeOnly();

        TextComponent component = new TextComponent()
            .SetDecoration(EDecorationFlags.None)
            .SetTextColor(EColorFlags.BrightBlack)
            .SetText(getPrefix(false) + name.Render())
            .UseLineBreak();

        return component;
    }

    private string getPrefix(bool bCurrentOrEnds)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
                return bCurrentOrEnds ? ">  " : "   ";
            case EOptionTypeFlags.MultiSelect:
                return IsSelected ? "[*] " : "[ ] ";
            default:
                return string.Empty;
        }
    }

    private EDecorationFlags getDecoration(bool bCurrentOrEnds)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                return bCurrentOrEnds ? EDecorationFlags.Bold : EDecorationFlags.None;
            default:
                return EDecorationFlags.None;
        }
    }

    private EColorFlags getColor(bool bCurrentOrEnds)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                return bCurrentOrEnds ? EColorFlags.Cyan : EColorFlags.Default;
            default:
                return EColorFlags.Default;
        }
    }
}
