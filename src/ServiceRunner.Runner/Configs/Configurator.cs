using ServiceRunner.Runner.Utils;

namespace ServiceRunner.Runner.Configs;

/// <include file='docs/configs/configurator.xml' path='Class/Description'/>
internal sealed class Configurator
{
    private readonly string mPath;
    internal readonly object Config;

    /// <include file='docs/configs/configurator.xml' path='Class/Constructor[@Name="Init"]'/>
    internal Configurator(string path)
    {
        mPath = path;
        Config = new object();
    }

    /// <include file='docs/configs/configurator.xml' path='Class/Constructor[@Name="WithConfig"]'/>
    internal Configurator(string path, object config)
    {
        mPath = path;
        Config = config;
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalStaticMethod[@Name="Parse"]'/>
    internal static Configurator Parse(string path)
    {
        using (ByteReader reader = new ByteReader(File.ReadAllBytes(path)))
        using (ConfigurationMetadata metadata = new ConfigurationMetadata(reader))
        {
            return new Configurator(path, metadata.Config);
        }
    }

    /// <include file='docs/configs/configurator.xml' path='Class/InternalMethod[@Name="SaveAsFile"]'/>
    internal void SaveAsFile()
    {
        using (ConfigurationMetadata metadata = new ConfigurationMetadata(Config))
        {
            byte[] bytes = metadata.ToBytes();

            File.WriteAllBytes(mPath, bytes);
        }
    }
}
