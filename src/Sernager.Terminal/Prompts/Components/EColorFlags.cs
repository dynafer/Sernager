namespace Sernager.Terminal.Prompts.Components;

[Flags]
internal enum EColorFlags
{
    Default = 0,
    Black = 1 << 0,
    Red = 1 << 1,
    Green = 1 << 2,
    Yellow = 1 << 3,
    Blue = 1 << 4,
    Magenta = 1 << 5,
    Cyan = 1 << 6,
    White = 1 << 7,
    BrightBlack = 1 << 8,
    BrightRed = 1 << 9,
    BrightGreen = 1 << 10,
    BrightYellow = 1 << 11,
    BrightBlue = 1 << 12,
    BrightMagenta = 1 << 13,
    BrightCyan = 1 << 14,
    BrightWhite = 1 << 15,
}
