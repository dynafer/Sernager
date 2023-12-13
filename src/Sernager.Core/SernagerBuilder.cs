using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Core.Tests")]
namespace Sernager.Core;

public class SernagerBuilder
{
    private string mConfigFilePath { get; set; } = string.Empty;

    public SernagerBuilder()
    {
    }

    public SernagerBuilder SetErrorLevel(EErrorLevel level)
    {
        ErrorManager.ErrorLevel = level;

        return this;
    }

    public SernagerBuilder UseConfig(string configPath)
    {
        if (!File.Exists(configPath))
        {
            ErrorManager.ThrowFail<FileNotFoundException>("Config file not found.", configPath);
        }
        else
        {
            Configurator.Parse(configPath);
        }

        return this;
    }

    public SernagerBuilder EnableAutoSave(EConfigurationType type = EConfigurationType.Sernager)
    {
        Configurator.UseAutoSave(type);

        return this;
    }

    public ISernagerService Build()
    {
        if (string.IsNullOrWhiteSpace(mConfigFilePath))
        {
            Configurator.Init();
        }
        else
        {
            Configurator.Parse(mConfigFilePath);
        }

        return new SernagerService();
    }
}
