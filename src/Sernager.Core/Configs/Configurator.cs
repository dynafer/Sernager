using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Utils;
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
            ErrorManager.ThrowFail<InvalidOperationException>("Configurator already initialized.");
            return;
        }

        mConfigDir = Path.GetFullPath(Directory.GetCurrentDirectory());
        Config = new Configuration();

        Debug.WriteLine("Configurator initialized.");
    }

    internal static void Parse(string filePath)
    {
        if (IsInitialized)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("Configurator already initialized.");
            return;
        }

        EConfigurationType? type = SernagerBuilderUtil.GetConfigurationTypeOrNull(filePath);

        if (type == null)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("Invalid configuration file extension.");
            return;
        }

        using (ByteReader reader = new ByteReader(File.ReadAllBytes(filePath)))
        using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type.Value))
        {
            mConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
            mConfigName = Path.GetFileNameWithoutExtension(filePath);
            Config = metadata.Config;
        }
    }

    internal static void UseAutoSave(EConfigurationType type = EConfigurationType.Sernager)
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            SaveAsFile(type);
        };
    }

    internal static void SaveAsFile(EConfigurationType type = EConfigurationType.Sernager)
    {
        if (!IsInitialized)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("Configurator hasn't initialized.");
            return;
        }

        string filePath = Path.Combine(mConfigDir, $"{mConfigName}{getExtension(type)}");

        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes(type);

            File.WriteAllBytes(filePath, bytes);
        }

        Debug.WriteLine($"Configuration saved to {filePath}.");
    }

    private static string getExtension(EConfigurationType type)
    {
        return type switch
        {
            EConfigurationType.Yaml => ".yaml",
            EConfigurationType.Json => ".json",
            EConfigurationType.Sernager => ".srn",
            _ => throw new InvalidOperationException("Invalid configuration type."),
        };
    }
}
