using Sernager.Terminal.Prompts;

namespace Sernager.Terminal;

internal static class Startup
{
    internal static void RegisterEvents()
    {
        EventHandler unloadEventHandler = (sender, e) => unloadCallback();
        ConsoleCancelEventHandler consoleUnloadEventHandler = (sender, e) => unloadCallback();

        AppDomain.CurrentDomain.DomainUnload += unloadEventHandler;
        AppDomain.CurrentDomain.ProcessExit += unloadEventHandler;
        Console.CancelKeyPress += consoleUnloadEventHandler;
    }

    private static void unloadCallback()
    {
        Prompter.Writer.Write(AnsiCode.EraseScreen());
        Prompter.Writer.Flush();
    }
}
