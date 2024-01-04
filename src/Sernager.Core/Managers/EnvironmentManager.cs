using Sernager.Core.Configs;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sernager.Core.Managers;

internal sealed class EnvironmentManager : IEnvironmentManager
{
    private EnvironmentModel mEnvrionmentGroup;

    internal EnvironmentManager(string name, string shortName, string description)
    {
        if (!Configurator.Config.EnvironmentGroups.ContainsKey(name))
        {
            EnvironmentModel environmentModel = new EnvironmentModel
            {
                Name = name,
                ShortName = shortName,
                Description = description
            };

            Configurator.Config.EnvironmentGroups.Add(name, environmentModel);
        }

        mEnvrionmentGroup = Configurator.Config.EnvironmentGroups[name];
    }

    public void RemoveGroup()
    {
        Configurator.Config.EnvironmentGroups.Remove(mEnvrionmentGroup.Name);
        mEnvrionmentGroup = null!;
    }

    public IEnvironmentManager AddPreFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        tryAddFile(mEnvrionmentGroup.PreVariables, filePath, option);

        return this;
    }

    public IEnvironmentManager AddFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        tryAddFile(mEnvrionmentGroup.Variables, filePath, option);

        return this;
    }

    public string? GetPreVariableOrNull(string key)
    {
        return getVariableOrNull(mEnvrionmentGroup.PreVariables, key);
    }

    public string? GetVariableOrNull(string key)
    {
        return getVariableOrNull(mEnvrionmentGroup.Variables, key);
    }

    public ReadOnlyDictionary<string, string> GetPreVariables()
    {
        return getVariables(mEnvrionmentGroup.PreVariables);
    }

    public ReadOnlyDictionary<string, string> GetVariables()
    {
        return getVariables(mEnvrionmentGroup.Variables);
    }

    public IEnvironmentManager SetPreVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        setVariable(mEnvrionmentGroup.PreVariables, key, value, option);

        return this;
    }

    public IEnvironmentManager SetVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        setVariable(mEnvrionmentGroup.Variables, key, value, option);

        return this;
    }

    public IEnvironmentManager SetPreVariables(Dictionary<string, string> variables, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(mEnvrionmentGroup.PreVariables, variable.Key, variable.Value, option);
        }

        return this;
    }

    public IEnvironmentManager SetVariables(Dictionary<string, string> variables, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(mEnvrionmentGroup.Variables, variable.Key, variable.Value, option);
        }

        return this;
    }

    public IEnvironmentManager RemovePreVariable(string key)
    {
        removeVariable(mEnvrionmentGroup.PreVariables, key);

        return this;
    }

    public IEnvironmentManager RemoveVariable(string key)
    {
        removeVariable(mEnvrionmentGroup.Variables, key);

        return this;
    }

    public IEnvironmentManager RemovePreVariables(params string[] keys)
    {
        foreach (string key in keys)
        {
            removeVariable(mEnvrionmentGroup.PreVariables, key);
        }

        return this;
    }

    public IEnvironmentManager RemoveVariables(params string[] keys)
    {
        foreach (string key in keys)
        {
            removeVariable(mEnvrionmentGroup.Variables, key);
        }

        return this;
    }

    private bool tryAddFile(Dictionary<string, string> target, string filePath, EAddDataOption option = EAddDataOption.SkipIfExists)
    {
        if (!File.Exists(filePath))
        {
            ExceptionManager.ThrowFail<FileNotFoundException>("Env file not found.", filePath);
            return false;
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

                string[] keyValue = line.Split('#')[0].Split('=');
                if (keyValue.Length != 2)
                {
                    Debug.WriteLine($"Invalid env variable format: {line}");

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

                setVariable(target, keyValue[0], keyValue[1], option);
            }
        }

        Debug.WriteLine($"Env file loaded: {filePath}");

        return true;
    }

    private string? getVariableOrNull(Dictionary<string, string> target, string key)
    {
        if (!target.ContainsKey(key))
        {
            return null;
        }

        return target[key];
    }

    private ReadOnlyDictionary<string, string> getVariables(Dictionary<string, string> target)
    {
        return new ReadOnlyDictionary<string, string>(target);
    }

    private void setVariable(Dictionary<string, string> target, string key, string value, EAddDataOption option)
    {
        switch (option)
        {
            case EAddDataOption.SkipIfExists:
                if (target.ContainsKey(key))
                {
                    return;
                }

                break;
            case EAddDataOption.Overwrite:
                if (target.ContainsKey(key))
                {
                    target.Remove(key);
                }

                break;
        }

        target.Add(key, value);
    }

    private void removeVariable(Dictionary<string, string> target, string key)
    {
        if (!target.ContainsKey(key))
        {
            return;
        }

        target.Remove(key);
    }
}
