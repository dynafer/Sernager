using ServiceRunner.Runner.Configs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServiceRunner.Runner.Tests")]
namespace ServiceRunner.Runner;

public class RunnerBuilder
{
    public RunnerBuilder(string configPath = "./config.sr")
    {
        if (string.IsNullOrWhiteSpace(configPath))
        {
            throw new ArgumentException($"Config path cannot be null or whitespace.");
        }

        if (!File.Exists(configPath))
        {
            Configurator.Parse(configPath);
        }
        else
        {
            Configurator.Init(configPath);
        }
    }
}
