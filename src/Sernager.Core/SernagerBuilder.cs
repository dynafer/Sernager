using Sernager.Core.Configs;
using Sernager.Core.Extensions;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics.CodeAnalysis;
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
        mConfigFilePath = configPath;

        return this;
    }

    [ExcludeFromCodeCoverage]
    public SernagerBuilder EnableAutoSave(EConfigurationType type)
    {
        Configurator.UseAutoSave(type);

        return this;
    }

    [ExcludeFromCodeCoverage]
    public SernagerBuilder EnableAutoSave(EUserFriendlyConfigurationType type)
    {
        Configurator.UseAutoSave(type);

        return this;
    }

    public ISernagerService Build()
    {
        if (string.IsNullOrWhiteSpace(mConfigFilePath) || !File.Exists(mConfigFilePath))
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
                    model.RemoveWhitespacesInDeclaredVariables(EEnvironmentType.Substitution, key);
                }

                foreach (string key in model.Variables.Keys)
                {
                    model.RemoveWhitespacesInDeclaredVariables(EEnvironmentType.Normal, key);
                }
            }
        }

        return new SernagerService();
    }
}
