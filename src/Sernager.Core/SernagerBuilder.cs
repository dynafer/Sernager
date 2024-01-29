using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sernager.Core.Tests")]
[assembly: InternalsVisibleTo("Sernager.Unit")]
namespace Sernager.Core;

public sealed class SernagerBuilder
{
    private string mConfigFilePath = string.Empty;

    public SernagerBuilder()
    {
    }

    public SernagerBuilder SetErrorLevel(EErrorLevel level)
    {
        ExceptionManager.ErrorLevel = level;

        return this;
    }

    public SernagerBuilder UseConfig(string configPath)
    {
        if (!File.Exists(configPath))
        {
            ExceptionManager.ThrowFail<FileNotFoundException>("Config file not found.", configPath);
        }
        else
        {
            mConfigFilePath = configPath;
        }

        return this;
    }

    public SernagerBuilder EnableAutoSave(EConfigurationType type)
    {
        Configurator.UseAutoSave(type);

        return this;
    }

    public SernagerBuilder EnableAutoSave(EUserFriendlyConfigurationType type)
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

            foreach (EnvironmentModel model in Configurator.Config.EnvironmentGroups.Values)
            {
                foreach (string key in model.SubstVariables.Keys)
                {
                    model.RemoveWhitespacesInDeclaredVariables(model.SubstVariables, key);
                }

                foreach (string key in model.Variables.Keys)
                {
                    model.RemoveWhitespacesInDeclaredVariables(model.Variables, key);
                }
            }
        }

        return new SernagerService();
    }
}
