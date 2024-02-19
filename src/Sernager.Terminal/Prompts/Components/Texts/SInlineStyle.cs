namespace Sernager.Terminal.Prompts.Components.Texts;

internal struct SInlineStyle
{
    internal static readonly string DECORATION_STYLE_NAME = "Decoration";
    internal static readonly string COLOR_STYLE_NAME = "Color";
    internal static readonly string RGB_COLOR_STYLE_NAME = "RgbColor";
    internal string Name { get; private init; }
    private object mValue;
    internal object Value
    {
        get
        {
            return mValue;
        }
    }

    internal SInlineStyle(string name, int value)
    {
        Name = name;
        mValue = value;
    }

    internal SInlineStyle(string name, int[] value)
    {
        foreach (int v in value)
        {
            if (v < 0 || v > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "RGB values must be between 0 and 255.");
            }
        }

        Name = name;
        mValue = value;
    }
}
