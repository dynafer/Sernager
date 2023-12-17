namespace Sernager.Terminal.Prompts;

internal static class PromptRenderer
{
    internal static TextWriter Writer { get; } = Console.Out;

    internal static void Render(List<string> prints)
    {
        for (int i = 0; i < prints.Count; ++i)
        {
            EraseLine();

            string paddedText = prints[i];

            Writer.WriteLine(paddedText);
        }
    }

    internal static void Render(params string[] prints)
    {
        Render(prints.ToList());
    }

    internal static void Write(string text)
    {
        Writer.Write(text);
    }

    internal static void WriteLine(string text)
    {
        Writer.WriteLine(text);
    }

    internal static void EraseScreen(int type = 0)
    {
        string ansiCode = AnsiCode.EraseScreen(type);

        Write(ansiCode);
    }

    internal static void EraseLine(int type = 0)
    {
        string ansiCode = AnsiCode.EraseLine(type);

        Write(ansiCode);
    }
}
