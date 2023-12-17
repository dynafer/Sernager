namespace Sernager.Terminal.Prompts;

internal static class PromptRenderer
{
    internal static TextWriter Writer { get; } = Console.Out;

    internal static void Render(List<string> prints)
    {
        for (int i = 0; i < prints.Count; ++i)
        {
            string paddedText = AddPadding(prints[i]);

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

    internal static string AddPadding(string text, int startX = 0)
    {
        string padding = new string(' ', Console.WindowWidth - startX - text.Length);

        return $"{text}{padding}";
    }
}
