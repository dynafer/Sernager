using Sernager.Runner;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Terminal.Tests")]

namespace Sernager.Terminal;

internal class Program
{
    static void Main()
    {
        RunnerBuilder builder = new RunnerBuilder();
        builder.EnableAutoSave();

        IService runner = builder.Build();

        runner.Setting.AddSettingName("test")
                    .UseManager("test")
                    .AddEnvFile("./bin/test.env");

        Console.WriteLine("Hello World!");
    }
}