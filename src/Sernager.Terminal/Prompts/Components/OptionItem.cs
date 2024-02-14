using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Extensions.Components;
using Sernager.Terminal.Prompts.Helpers;
using Sernager.Terminal.Prompts.Plugins;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class OptionItem<T>
    where T : notnull
{
    private readonly EOptionTypeFlags mType;
    private readonly bool mbUseResourcePack;
    internal string Name { get; init; }
    internal T Value { get; init; }
    internal bool IsSelected { get; private set; }

    internal OptionItem(EOptionTypeFlags type, string name, T value, bool bUseResourcePack)
    {
        mType = type;
        mbUseResourcePack = bUseResourcePack;
        Name = name;
        Value = value;
        IsSelected = false;
    }

    internal void ToggleSelection()
    {
        IsSelected = !IsSelected;
    }

    internal TextComponent ToTextComponent(IBasePlugin plugin, bool bCurrentOrEnd)
    {
        string name = mbUseResourcePack
            ? PluginResourceHelper.GetString(plugin, Name)
            : Name;

        IPromptComponent nameComponent = new InlineStyledTextComponent()
            .SetText(name);

        TextComponent component = new TextComponent()
            .SetDecoration(getDecoration(bCurrentOrEnd))
            .SetTextColor(getColor(bCurrentOrEnd))
            .SetText(getPrefix(bCurrentOrEnd) + nameComponent.Render())
            .UseLineBreak();

        return component;
    }

    internal TextComponent ToRestTextComponent(IBasePlugin plugin)
    {
        string name = mbUseResourcePack
            ? PluginResourceHelper.GetString(plugin, Name)
            : Name;

        IPromptComponent nameComponent = new InlineStyledTextComponent()
            .SetText(name)
            .UsePlainTextOnly();

        TextComponent component = new TextComponent()
            .SetDecoration(EDecorationFlags.None)
            .SetTextColor(EColorFlags.BrightBlack)
            .SetText(getPrefix(false) + nameComponent.Render())
            .UseLineBreak();

        return component;
    }

    private string getPrefix(bool bCurrentOrEnd)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
                return bCurrentOrEnd ? ">  " : "   ";
            case EOptionTypeFlags.MultiSelect:
                return IsSelected ? "[*] " : "[ ] ";
            default:
                return string.Empty;
        }
    }

    private EDecorationFlags getDecoration(bool bCurrentOrEnd)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                if (bCurrentOrEnd)
                {
                    return EDecorationFlags.Bold;
                }
                else
                {
                    return EDecorationFlags.None;
                }
            default:
                return EDecorationFlags.None;
        }
    }

    private EColorFlags getColor(bool bCurrentOrEnd)
    {
        switch (mType)
        {
            case EOptionTypeFlags.Select:
            case EOptionTypeFlags.MultiSelect:
                if (bCurrentOrEnd)
                {
                    return EColorFlags.Cyan;
                }
                else
                {
                    return EColorFlags.Default;
                }
            default:
                return EColorFlags.Default;
        }
    }
}
