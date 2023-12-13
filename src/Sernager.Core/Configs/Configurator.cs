using Sernager.Core.Managers;
using Sernager.Core.Options;
using Sernager.Core.Utils;

namespace Sernager.Core.Configs;

internal static class Configurator
{
    private static string mConfigDir { get; set; } = null!;
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
    }

    internal static void Parse(string filePath)
    {
        if (IsInitialized)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("Configurator already initialized.");
            return;
        }

        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        EConfigurationType? type = extension switch
        {
            ".yml" => EConfigurationType.Yaml,
            ".yaml" => EConfigurationType.Yaml,
            ".json" => EConfigurationType.Json,
            ".srn" => EConfigurationType.Sernager,
            _ => null,
        };

        if (type == null)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("Invalid configuration file extension.");
            return;
        }

        using (ByteReader reader = new ByteReader(File.ReadAllBytes(filePath)))
        using (ConfigurationMetadata metadata = ConfigurationMetadata.Parse(reader, type.Value))
        {
            mConfigDir = Path.GetFullPath(Path.GetDirectoryName(filePath) ?? "./");
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

        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes(type);

            File.WriteAllBytes(mConfigDir, bytes);
        }
    }
}
