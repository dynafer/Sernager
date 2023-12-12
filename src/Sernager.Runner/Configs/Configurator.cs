using Sernager.Runner.Utils;

namespace Sernager.Runner.Configs;

/// <include file='docs/configs/configurator.xml' path='Class/Description'/>
internal static class Configurator
{
    private static string mPath { get; set; } = null!;
    internal static Configuration Config { get; private set; } = null!;
    internal static bool IsInitialized => mPath != null && Config != null;

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="Init"]'/>
    internal static void Init(string path)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Configurator already initialized");
        }

        mPath = path;
        Config = new Configuration();
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="Parse"]'/>
    internal static void Parse(string path)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Configurator already initialized");
        }

        using (ByteReader reader = new ByteReader(File.ReadAllBytes(path)))
        using (ConfigurationMetadata metadata = new ConfigurationMetadata(reader))
        {
            mPath = path;
            Config = metadata.Config;
        }
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="UseAutoSave"]'/>
    internal static void UseAutoSave()
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            SaveAsFile();
        };
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="SaveAsFile"]'/>
    internal static void SaveAsFile()
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("Configurator not initialized");
        }

        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes();

            File.WriteAllBytes(mPath, bytes);
        }
    }
}
