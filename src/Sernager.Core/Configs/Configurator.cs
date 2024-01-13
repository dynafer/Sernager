using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Utils;
using System.ComponentModel;
using System.Diagnostics;

namespace Sernager.Core.Configs;

internal static class Configurator
{
    private static string mConfigDir = null!;
    private static string mConfigName = null!;
    internal static Configuration Config { get; private set; } = null!;
    internal static bool IsInitialized => mConfigDir != null && Config != null;

    internal static void Init()
    {
        if (IsInitialized)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("Configurator already initialized.");
            return;
        }

        mConfigDir = Path.GetFullPath(Directory.GetCurrentDirectory());
        mConfigName = "sernager";
        Config = new Configuration();

        Debug.WriteLine("Configurator initialized.");
    }

    internal static void Parse(string filePath)
    {
        if (IsInitialized)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("Configurator already initialized.");
            return;
        }

        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(filePath);

        if (type == null)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("Invalid configuration file extension.");
            return;
        }

        using (ByteReader reader = new ByteReader(File.ReadAllBytes(filePath)))
        using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type.Value))
        {
            string currentDirectoryName = Path.GetDirectoryName(filePath) ?? "./";
            if (string.IsNullOrWhiteSpace(currentDirectoryName))
            {
                currentDirectoryName = "./";
            }

            mConfigDir = Path.GetFullPath(currentDirectoryName);
            mConfigName = Path.GetFileNameWithoutExtension(filePath).Replace(".default", "").Replace(".userfriendly", "");
            Config = metadata.Config;
        }
    }

    internal static void UseAutoSave(EConfigurationType type)
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            SaveAsFile(type);
        };
    }

    internal static void UseAutoSave(EUserFriendlyConfigurationType type)
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            SaveAsFile(type);
        };
    }

    internal static void SaveAsFile(EConfigurationType type)
    {
        if (!IsInitialized)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("Configurator hasn't initialized.");
            return;
        }

        string filePath = Path.Combine(mConfigDir, $"{mConfigName}.default{GetExtension(type)}");

        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes(type);

            File.WriteAllBytes(filePath, bytes);
        }

        Debug.WriteLine($"Configuration saved to {filePath}.");
    }

    internal static void SaveAsFile(EUserFriendlyConfigurationType type)
    {
        if (!IsInitialized)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("Configurator hasn't initialized.");
            return;
        }

        string filePath = Path.Combine(mConfigDir, $"{mConfigName}.userfriendly{GetExtension(type)}");

        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes(type);

            File.WriteAllBytes(filePath, bytes);
        }

        Debug.WriteLine($"Configuration saved to {filePath}.");
    }

    internal static string GetExtension(EConfigurationType type)
    {
        return type switch
        {
            EConfigurationType.Yaml => ".yaml",
            EConfigurationType.Json => ".json",
            EConfigurationType.Sernager => ".srn",
            _ => throw new InvalidEnumArgumentException("Invalid configuration type."),
        };
    }

    internal static string GetExtension(EUserFriendlyConfigurationType type)
    {
        return type switch
        {
            EUserFriendlyConfigurationType.Yaml => ".yaml",
            EUserFriendlyConfigurationType.Json => ".json",
            _ => throw new InvalidEnumArgumentException("Invalid configuration type."),
        };
    }
}
