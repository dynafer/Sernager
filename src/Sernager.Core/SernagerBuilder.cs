using Sernager.Core.Configs;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Core.Tests")]
namespace Sernager.Core;

public class SernagerBuilder
{
    public SernagerBuilder(string configPath = "./config.sr")
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

    public SernagerBuilder EnableAutoSave()
    {
        Configurator.UseAutoSave();

        return this;
    }

    public ISernagerService Build()
    {
        return new SernagerService();
    }
}
