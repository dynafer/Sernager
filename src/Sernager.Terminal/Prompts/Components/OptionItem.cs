namespace Sernager.Terminal.Prompts.Components;

internal sealed class OptionItem
{
    private readonly EOptionTypeFlags mType = EOptionTypeFlags.None;
    internal string Name { get; init; } = string.Empty;
    internal string Value { get; init; } = string.Empty;
    internal bool IsSelected { get; private set; } = false;

    internal OptionItem(EOptionTypeFlags type, string name)
    {
        mType = type;
        Name = name;
        Value = name;
    }

    internal OptionItem(EOptionTypeFlags type, string name, string value)
    {
        mType = type;
        Name = name;
        Value = value;
    }

    internal void Select()
    {
        IsSelected = true;
    }

    internal void Deselect()
    {
        IsSelected = false;
    }

    internal TextComponent ToTextComponent(bool bCurrentOrEnds = false)
    {
        TextComponent component = new TextComponent()
            .SetDecoration(getDecoration(bCurrentOrEnds))
            .SetTextColor(getColor(bCurrentOrEnds))
            .SetText(getPrefix(bCurrentOrEnds) + Name)
            .UseLineBreak();

        return component;
    }

    internal TextComponent ToRestTextComponent()
    {
        TextComponent component = new TextComponent()
            .SetDecoration(EDecorationFlags.None)
            .SetTextColor(EColorFlags.BrightBlack)
            .SetText(getPrefix(false) + Name)
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
                throw new InvalidOperationException();
        }
    }
}
