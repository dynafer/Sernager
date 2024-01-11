using Sernager.Terminal.Prompts;
using Sernager.Terminal.Prompts.Components;
using Sernager.Terminal.Prompts.Extensions.Components;

namespace Sernager.Terminal;

internal static class Logger
{
    internal static void Log(params string[] messages)
    {
        using (Renderer renderer = new Renderer(Prompter.Writer))
        {
            List<IPromptComponent> components = new List<IPromptComponent>();

            foreach (string message in messages)
            {
                components.Add(
                    new InlineStyledTextComponent()
                        .SetText(message)
                );
            }

            renderer.Render(components);
        }
    }

    internal static void Error(params string[] messages)
    {
        using (Renderer renderer = new Renderer(Prompter.Writer))
        {
            List<IPromptComponent> components = new List<IPromptComponent>();

            foreach (string message in messages)
            {
                components.Add(
                    new InlineStyledTextComponent()
                        .SetText($"[Red]{message}[/Red]")
                );
            }

            renderer.Render(components);
        }
    }

    internal static void ErrorWithExit(params string[] messages)
    {
        Error(messages);
        Environment.Exit(1);
    }
}
