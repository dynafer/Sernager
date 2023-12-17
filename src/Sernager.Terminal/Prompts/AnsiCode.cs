namespace Sernager.Terminal.Prompts;

internal readonly record struct AnsiCode
{
    internal const string ESCAPE_CSI = "\u001b[";

    internal static string CursorUp(int amount = 1)
    {
        return $"{ESCAPE_CSI}{amount}A";
    }

    internal static string CursorDown(int amount = 1)
    {
        return $"{ESCAPE_CSI}{amount}B";
    }

    internal static string CursorRight(int amount = 1)
    {
        return $"{ESCAPE_CSI}{amount}C";
    }

    internal static string CursorLeft(int amount = 1)
    {
        return $"{ESCAPE_CSI}{amount}D";
    }

    internal static string CursorHorizontalAbsolute(int amount = 1)
    {
        return $"{ESCAPE_CSI}{amount}G";
    }

    internal static string EraseScreen(int type = 0)
    {
        return $"{ESCAPE_CSI}{type}J";
    }

    internal static string EraseLine(int type = 0)
    {
        return $"{ESCAPE_CSI}{type}K";
    }

    internal static string GraphicsMode(params int[] codes)
    {
        return $"{ESCAPE_CSI}{string.Join(";", codes)}m";
    }

    internal static string ResetGraphicsMode()
    {
        return $"{ESCAPE_CSI}0m";
    }
}
