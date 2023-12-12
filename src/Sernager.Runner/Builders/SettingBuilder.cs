using Sernager.Runner.Configs;
using Sernager.Runner.Managers;
using Sernager.Runner.Options;

namespace Sernager.Runner.Builders;

/// <include file='docs/builders/setting_builder.xml' path='Class/Description'/> 
public class SettingBuilder : ISettingBuilder
{
    private readonly Dictionary<string, SettingManager> mSettingManagers = new Dictionary<string, SettingManager>();

    /// <include file='docs/builders/setting_builder.xml' path='Class/Constructor'/>
    public SettingBuilder()
    {
        if (!Configurator.IsInitialized)
        {
            throw new InvalidOperationException("You must call RunnerBuilder.Build().");
        }
    }

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingName"]'/>
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
                case EAddDataOption.ThrowIfExists:
                    throw new InvalidOperationException($"Setting name '{name}' already exists.");
            }
        }

        Configurator.Config.SettingNames.Add(name);

        return this;
    }

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingNames"][@Type="WithDefaultOption"]'/>
    public ISettingBuilder AddSettingNames(params string[] names)
    {
        foreach (string name in names)
        {
            AddSettingName(name, EAddDataOption.SkipIfExists);
        }

        return this;
    }

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="AddSettingNames"][@Type="WithGivenOption"]'/>
    public ISettingBuilder AddSettingNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (string name in names)
        {
            AddSettingName(name, option);
        }

        return this;
    }

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="RemoveSettingName"]'/>
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

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="RemoveSettingNames"]'/>
    public ISettingBuilder RemoveSettingNames(params string[] names)
    {
        foreach (string name in names)
        {
            RemoveSettingName(name);
        }

        return this;
    }

    /// <include file='docs/builders/setting_builder.xml' path='Class/PublicMethod[@Name="UseManager"]'/>
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
