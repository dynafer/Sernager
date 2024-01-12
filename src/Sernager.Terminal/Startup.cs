using Sernager.Core;
using Sernager.Core.Managers;
using Sernager.Core.Utils;
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

    internal static ISernagerService InitializeSernager()
    {
        SernagerBuilder builder = new SernagerBuilder();

        if (Args.Model.ConfigPath != null)
        {
            builder.UseConfig(Args.Model.ConfigPath);
        }
        else
        {
            IEnumerable<string> files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                if (SernagerBuilderUtil.GetConfigurationTypeOrNull(file) != null)
                {
                    builder.UseConfig(file);
                    break;
                }
            }
        }

        if (Args.Model.AutoSaveType != null)
        {
            builder.EnableAutoSave(Args.Model.AutoSaveType.Value);
        }

        if (Args.Model.AutoSaveUserFriendlyType != null)
        {
            builder.EnableAutoSave(Args.Model.AutoSaveUserFriendlyType.Value);
        }

        ExceptionManager.ErrorLevel = Args.Model.ErrorLevel;

        Args.Complete();

        return builder.Build();
    }

    private static void unloadCallback()
    {
        if (Prompter.CountRemovableConsoleLines > 0)
        {
            Prompter.Writer.Write(AnsiCode.CursorUp(Prompter.CountRemovableConsoleLines));
        }

        Prompter.Writer.Write(AnsiCode.EraseScreen());
        Prompter.Writer.Flush();
    }
}
