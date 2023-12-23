using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Terminal.Tests")]

namespace Sernager.Terminal;

internal class Program
{
    internal static void Main(params string[] args)
    {
        Args.Init();
        Args.Parse(args);

        Startup.RegisterEvents();
    }
}