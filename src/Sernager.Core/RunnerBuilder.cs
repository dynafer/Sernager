using Sernager.Core.Configs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Core.Tests")]
namespace Sernager.Core;

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
            Configurator.Init(configPath);
        }
        else
        {
            Configurator.Parse(configPath);
        }
    }

    public RunnerBuilder EnableAutoSave()
    {
        Configurator.UseAutoSave();

        return this;
    }

    public IService Build()
    {
        return new Service();
    }
}
