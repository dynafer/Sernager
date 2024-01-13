using Sernager.Core;
using Sernager.Terminal.Managers;
using Sernager.Terminal.Models;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Sernager.Terminal.Tests")]
[assembly: InternalsVisibleTo("Sernager.Unit")]
namespace Sernager.Terminal;

internal class Program
{
    internal static ISernagerService Service { get; private set; } = null!;

    internal static void Main(params string[] args)
    {
        Console.InputEncoding = Encoding.Default;
        Console.OutputEncoding = Encoding.Default;

        Startup.RegisterEvents();

        Args.Parse(args);

        EManagementTypeFlags? managementType = Args.GetManagementType();
        string[] commands = Args.GetCommands(args);

        Service = Startup.InitializeSernager();

        FlowManager.Start(managementType, commands);
    }
}