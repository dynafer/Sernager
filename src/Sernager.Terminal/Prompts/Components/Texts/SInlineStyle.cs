namespace Sernager.Terminal.Prompts.Components.Texts;

internal struct SInlineStyle
{
    internal string Name { get; private init; }
    private object mValue;
    internal object Value
    {
        get
        {
            return mValue;
        }
        private init
        {
            mValue = value;
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
