namespace Sernager.Terminal.Prompts.Styles;

[Flags]
internal enum EPromptDecorationFlags
{
    None = 0,
    Bold = 1 << 0,
    Dim = 1 << 1,
    Italic = 1 << 2,
    Underline = 1 << 3,
    Hidden = 1 << 4,
    Strikethrough = 1 << 5,
}
