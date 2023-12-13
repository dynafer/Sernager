using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Builders;

internal class SettingBuilder : ISettingBuilder
{
    private readonly Dictionary<string, SettingManager> mSettingManagers = new Dictionary<string, SettingManager>();

    internal SettingBuilder()
    {
        if (!Configurator.IsInitialized)
        {
            ErrorManager.ThrowFail<InvalidOperationException>("You must call SernagerBuilder.Build().");
            return;
        }
    }

    public ISettingBuilder AddSettingName(string name, EAddDataOption option = EAddDataOption.SkipIfExists)
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

    public ISettingBuilder AddSettingNames(params string[] names)
    {
        foreach (string name in names)
        {
            AddSettingName(name, EAddDataOption.SkipIfExists);
        }

        return this;
    }

    public ISettingBuilder AddSettingNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (string name in names)
        {
            AddSettingName(name, option);
        }

        return this;
    }

    public ISettingBuilder RemoveSettingName(string name)
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

    public ISettingBuilder RemoveSettingNames(params string[] names)
    {
        foreach (string name in names)
        {
            RemoveSettingName(name);
        }

        return this;
    }

    public ISettingManager UseManager(string name)
    {
        if (!Configurator.Config.SettingNames.Contains(name))
        {
            AddSettingName(name);
        }

        if (!mSettingManagers.ContainsKey(name))
        {
            mSettingManagers.Add(name, new SettingManager(name));
        }

        return mSettingManagers[name];
    }
}
