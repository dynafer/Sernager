using Sernager.Terminal.Prompts.Components.Texts;
using Sernager.Terminal.Prompts.Helpers;

namespace Sernager.Terminal.Prompts.Components;

internal sealed class TextComponent : IPromptComponent
{
    internal EDecorationFlags Decoration { get; set; } = EDecorationFlags.None;
    internal EColorFlags TextColor { get; set; } = EColorFlags.Default;
    internal RgbColor? RgbColor { get; set; } = null;
    internal string Text { get; set; } = string.Empty;
    public bool IsLineBreak { get; set; } = false;

    internal TextComponent UseLineBreak()
    {
        IsLineBreak = true;

        return this;
    }

    string IPromptComponent.Render()
    {
        List<int> codes = new List<int>();

        if (Decoration != EDecorationFlags.None)
        {
            codes.Add(AnsiCodeHelper.FromDecoration(Decoration));
        }

        if (TextColor != EColorFlags.Default)
        {
            codes.Add(AnsiCodeHelper.FromTextColor(TextColor));
        }

        if (RgbColor.HasValue)
        {
            codes.AddRange(AnsiCodeHelper.FromTextRgbColor(RgbColor));
        }

        return $"{AnsiCode.GraphicsMode(codes.ToArray())}{Text}{AnsiCode.ResetGraphicsMode()}";
    }
}
