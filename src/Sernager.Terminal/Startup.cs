using Sernager.Terminal.Prompts;

namespace Sernager.Terminal;

internal static class Startup
{
    internal static void RegisterEvents()
    {
        EventHandler unloadEventHandler = (sender, e) => UnloadCallback();
        ConsoleCancelEventHandler consoleUnloadEventHandler = (sender, e) => UnloadCallback();

        AppDomain.CurrentDomain.DomainUnload += unloadEventHandler;
        AppDomain.CurrentDomain.ProcessExit += unloadEventHandler;
        Console.CancelKeyPress += consoleUnloadEventHandler;
    }

    internal static void UnloadCallback()
    {
        Prompter.Writer.Write(AnsiCode.EraseScreen());
        Prompter.Writer.Flush();
    }
}
