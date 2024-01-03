using Sernager.Core;
using Sernager.Terminal.Managers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Terminal.Tests")]

namespace Sernager.Terminal;

internal class Program
{
    internal static ISernagerService Service { get; private set; } = null!;

    internal static void Main(params string[] args)
    {
        Startup.RegisterEvents();

        string[] commands = Args.GetCommands(args);

        Args.Init();
        Args.Parse(args);

        Service = Startup.InitializeSernager();

        FlowManager.Start(commands);
    }
}