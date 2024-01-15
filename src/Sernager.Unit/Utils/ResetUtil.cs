using Sernager.Core.Configs;
using System.Reflection;

namespace Sernager.Unit.Utils;

public static class ResetUtil
{
    public static void ResetConfigurator()
    {
        Type type = typeof(Configurator);
        FieldInfo? mConfigDir = type.GetField("mConfigDir", BindingFlags.Static | BindingFlags.NonPublic);
        if (mConfigDir == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} 'mConfigDir' is null.");
        }

        FieldInfo? mConfigName = type.GetField("mConfigName", BindingFlags.Static | BindingFlags.NonPublic);
        if (mConfigName == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} 'mConfigName' is null.");
        }

        PropertyInfo? Config = type.GetProperty("Config", BindingFlags.Static | BindingFlags.NonPublic);
        if (Config == null)
        {
            throw new InvalidOperationException($"{nameof(FieldInfo)} 'Config' is null.");
        }

        mConfigDir.SetValue(null, null);
        mConfigName.SetValue(null, null);
        Config.SetValue(null, null);
    }
}
