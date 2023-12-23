namespace Sernager.Terminal.Prompts.Components.Texts;

[Flags]
internal enum EDecorationFlags
{
    None = 0,
    Bold = 1 << 0,
    Dim = 1 << 1,
    Italic = 1 << 2,
    Underline = 1 << 3,
    Hidden = 1 << 4,
    Strikethrough = 1 << 5,
}
