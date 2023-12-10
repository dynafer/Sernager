using ServiceRunner.Runner.Utils;
using System.Diagnostics;

namespace ServiceRunner.Runner.Configs;

/// <include file='docs/configs/configurator.xml' path='Class/Description'/>
internal static class Configurator
{
    private static string mPath { get; set; } = null!;
    internal static object Config { get; private set; } = null!;

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="Init"]'/>
    internal static void Init(string path)
    {
        if (mPath != null)
        {
            Debug.WriteLine("Configurator already initialized");
            return;
        }

        mPath = path;
        Config = new object();
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="Parse"]'/>
    internal static void Parse(string path)
    {
        if (mPath != null)
        {
            Debug.WriteLine("Configurator already initialized");
            return;
        }

        using (ByteReader reader = new ByteReader(File.ReadAllBytes(path)))
        using (ConfigurationMetadata metadata = new ConfigurationMetadata(reader))
        {
            mPath = path;
            Config = metadata.Config;
        }
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="SaveAsFile"]'/>
    internal static void SaveAsFile()
    {
        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes();

            File.WriteAllBytes(mPath, bytes);
        }
    }
}
