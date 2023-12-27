using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Builders;

internal sealed class SettingBuilder : ISettingBuilder
{
    private readonly Dictionary<string, SettingManager> mSettingManagers = new Dictionary<string, SettingManager>();

    internal SettingBuilder()
    {
        if (!Configurator.IsInitialized)
        {
            ExceptionManager.ThrowFail<InvalidOperationException>("You must call SernagerBuilder.Build().");
            return;
        }
    }

    public ISettingBuilder AddName(string name, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        if (Configurator.Config.SettingNames.Contains(name))
        {
            switch (option)
            {
                case EAddDataOption.SkipIfExists:
                    return this;
                case EAddDataOption.Overwrite:
                    Configurator.Config.SettingNames.Remove(name);
                    break;
            }
        }

        Configurator.Config.SettingNames.Add(name);

        return this;
    }

    public ISettingBuilder AddNames(params string[] names)
    {
        foreach (string name in names)
        {
            AddName(name, EAddDataOption.SkipIfExists);
        }

        return this;
    }

    public ISettingBuilder AddNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (string name in names)
        {
            AddName(name, option);
        }

        return this;
    }

    public ISettingBuilder RemoveName(string name)
    {
        if (!Configurator.Config.SettingNames.Contains(name))
        {
            return this;
        }

        Configurator.Config.SettingNames.Remove(name);

        if (mSettingManagers.ContainsKey(name))
        {
            mSettingManagers.Remove(name);
        }

        if (Configurator.Config.Settings.ContainsKey(name))
        {
            Configurator.Config.Settings.Remove(name);
        }

        return this;
    }

    public ISettingBuilder RemoveNames(params string[] names)
    {
        foreach (string name in names)
        {
            RemoveName(name);
        }

        return this;
    }

    public ISettingManager UseManager(string name)
    {
        if (!Configurator.Config.SettingNames.Contains(name))
        {
            AddName(name);
        }

        if (!mSettingManagers.ContainsKey(name))
        {
            mSettingManagers.Add(name, new SettingManager(name));
        }

        return mSettingManagers[name];
    }
}
