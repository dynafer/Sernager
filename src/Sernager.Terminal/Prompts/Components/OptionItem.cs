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

    internal TextComponent ToTextComponent(bool bCurrent = false)
    {
        TextComponent component = new TextComponent()
            .SetDecoration(getDecoration(bCurrent))
            .SetTextColor(getColor(bCurrent))
            .SetText(getPrefix(bCurrent) + Name);

        if (mType != EOptionTypeFlags.Hint)
        {
            component.UseLineBreak();
        }

        return component;
    }

    internal TextComponent ToRestTextComponent()
    {
        TextComponent component = new TextComponent()
            .SetDecoration(EDecorationFlags.None)
            .SetTextColor(EColorFlags.BrightBlack)
            .SetText(getPrefix(false) + Name);

        if (mType != EOptionTypeFlags.Hint)
        {
            component.UseLineBreak();
        }

        return component;
    }

    private string getPrefix(bool bCurrent)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
                return bCurrent ? ">  " : "   ";
            case EOptionTypeFlags.MultiSelect:
                return IsSelected ? "[*] " : "[ ] ";
            default:
                return string.Empty;
        }
    }

    private EDecorationFlags getDecoration(bool bCurrent)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                return bCurrent ? EDecorationFlags.Bold : EDecorationFlags.None;
            default:
                return EDecorationFlags.None;
        }
    }

    private EColorFlags getColor(bool bCurrent)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                return bCurrent ? EColorFlags.Cyan : EColorFlags.Default;
            case EOptionTypeFlags.Hint:
                return EColorFlags.BrightBlack;
            default:
                throw new InvalidOperationException();
        }
    }
}
