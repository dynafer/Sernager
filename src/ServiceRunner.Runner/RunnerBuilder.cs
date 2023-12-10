using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ServiceRunner.Runner.Tests")]
namespace ServiceRunner.Runner;

public class RunnerBuilder
{
    public RunnerBuilder(string configPath = "config.sr")
    {
    }
}
