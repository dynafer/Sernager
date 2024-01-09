namespace Sernager.Terminal.Prompts;

internal static class AnsiCode
{
    internal const string ESCAPE_CSI = "\u001b[";

    internal static string CursorUp(int count = 1)
    {
        return $"{ESCAPE_CSI}{count}A";
    }

    internal static string CursorDown(int count = 1)
    {
        return $"{ESCAPE_CSI}{count}B";
    }

    internal static string CursorRight(int count = 1)
    {
        return $"{ESCAPE_CSI}{count}C";
    }

    internal static string CursorLeft(int count = 1)
    {
        return $"{ESCAPE_CSI}{count}D";
    }

    internal static string CursorHorizontalAbsolute(int count = 1)
    {
        return $"{ESCAPE_CSI}{count}G";
    }

    internal static string ShowCursor()
    {
        return $"{ESCAPE_CSI}?25h";
    }

    internal static string HideCursor()
    {
        return $"{ESCAPE_CSI}?25l";
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
