namespace Sernager.Terminal.Prompts.Plugins.Utilities;

internal struct SCursor
{
    internal int X { get; set; } = 0;
    internal int Y { get; set; } = 0;

    internal SCursor(int x, int y)
    {
        X = x;
        Y = y;
    }
}
