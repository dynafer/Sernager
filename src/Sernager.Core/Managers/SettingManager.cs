using Sernager.Core.Configs;
using Sernager.Core.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sernager.Core.Managers;

/// <include file='docs/managers/setting_manager.xml' path='Class/Description'/> 
internal class SettingManager : ISettingManager
{
    private Dictionary<string, string> mSettings { get; set; }

    /// <include file='docs/managers/setting_manager.xml' path='Class/Constructor'/>
    internal SettingManager(string name)
    {
        if (!Configurator.Config.Settings.ContainsKey(name))
        {
            Configurator.Config.Settings.Add(name, new Dictionary<string, string>());
        }

        mSettings = Configurator.Config.Settings[name];
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="AddEnvFile"]'/>
    public ISettingManager AddEnvFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        if (!File.Exists(filePath))
        {
            if (option == EAddDataOption.ThrowIfExists)
            {
                throw new FileNotFoundException($"File '{filePath}' does not exist.");
            }
            else
            {
                Debug.WriteLine($"File '{filePath}' does not exist.");

                return this;
            }
        }

        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(stream))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith('#'))
                {
                    continue;
                }

                string[] keyValue = line.Split('=');
                if (keyValue.Length != 2)
                {
                    Debug.WriteLine($"Invalid env file format: {line}");

                    continue;
                }

                keyValue[0] = keyValue[0].Trim();
                keyValue[1] = keyValue[1].Split('#')[0].Trim();

                if (keyValue[1].StartsWith('"') && keyValue[1].EndsWith('"')
                    || keyValue[1].StartsWith('\'') && keyValue[1].EndsWith('\'')
                )
                {
                    keyValue[1] = keyValue[1].Substring(1, keyValue[1].Length - 2);
                }

                setEnvData(keyValue[0], keyValue[1], option);
            }
        }

        return this;
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="GetEnvVariableOrNull"]'/>
    public string? GetEnvVariableOrNull(string key)
    {
        if (!mSettings.ContainsKey(key))
        {
            return null;
        }

        return mSettings[key];
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="GetEnvVariables"]'/>
    public ReadOnlyDictionary<string, string> GetEnvVariables()
    {
        return new ReadOnlyDictionary<string, string>(mSettings);
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="SetEnvVariable"]'/>
    public ISettingManager SetEnvVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        setEnvData(key, value, option);

        return this;
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="SetEnvVariables"]'/>
    public ISettingManager SetEnvVariables(IDictionary<string, string> variables, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (KeyValuePair<string, string> variable in variables)
        {
            setEnvData(variable.Key, variable.Value, option);
        }

        return this;
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="RemoveEnvVariable"]'/>
    public ISettingManager RemoveEnvVariable(string key)
    {
        if (!mSettings.ContainsKey(key))
        {
            return this;
        }

        mSettings.Remove(key);

        return this;
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PublicMethod[@Name="RemoveEnvVariables"]'/>
    public ISettingManager RemoveEnvVariables(params string[] keys)
    {
        foreach (string key in keys)
        {
            if (!mSettings.ContainsKey(key))
            {
                continue;
            }

            mSettings.Remove(key);
        }

        return this;
    }

    /// <include file='docs/managers/setting_manager.xml' path='Class/PrivateMethod[@Name="setEnvData"]'/>
    private void setEnvData(string key, string value, EAddDataOption option)
    {
        switch (option)
        {
            case EAddDataOption.SkipIfExists:
                if (mSettings.ContainsKey(key))
                {
                    return;
                }

                break;
            case EAddDataOption.Overwrite:
                if (mSettings.ContainsKey(key))
                {
                    mSettings.Remove(key);
                }

                break;
            case EAddDataOption.ThrowIfExists:
                if (mSettings.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Key '{key}' already exists.");
                }

                break;
        }

        mSettings.Add(key, value);
    }
}
