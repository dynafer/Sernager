using Sernager.Core.Configs;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sernager.Core.Managers;

internal sealed class EnvironmentManager : IEnvironmentManager
{
    public EnvironmentModel EnvironmentGroup { get; private set; }
    public EAddDataOption AdditionMode { get; set; } = EAddDataOption.SkipIfExists;

    internal EnvironmentManager(string name)
    {
        if (!Configurator.Config.EnvironmentGroups.ContainsKey(name))
        {
            EnvironmentModel environmentModel = new EnvironmentModel
            {
                Name = name
            };

            Configurator.Config.EnvironmentGroups.Add(name, environmentModel);
        }

        EnvironmentGroup = Configurator.Config.EnvironmentGroups[name];
    }

    public void RemoveGroup()
    {
        Configurator.Config.EnvironmentGroups.Remove(EnvironmentGroup.Name);
        EnvironmentGroup = null!;
    }

    public IEnvironmentManager AddFromPreFile(string filePath)
    {
        if (AdditionMode == EAddDataOption.OverwriteAll)
        {
            EnvironmentGroup.PreVariables.Clear();
        }

        tryAddFromFile(EnvironmentGroup.PreVariables, filePath);

        return this;
    }

    public IEnvironmentManager AddFromFile(string filePath)
    {
        if (AdditionMode == EAddDataOption.OverwriteAll)
        {
            EnvironmentGroup.Variables.Clear();
        }

        tryAddFromFile(EnvironmentGroup.Variables, filePath);

        return this;
    }

    public string? GetPreVariableOrNull(string key)
    {
        return getVariableOrNull(EnvironmentGroup.PreVariables, key);
    }

    public string? GetVariableOrNull(string key)
    {
        return getVariableOrNull(EnvironmentGroup.Variables, key);
    }

    public ReadOnlyDictionary<string, string> GetPreVariables()
    {
        return getVariables(EnvironmentGroup.PreVariables);
    }

    public ReadOnlyDictionary<string, string> GetVariables()
    {
        return getVariables(EnvironmentGroup.Variables);
    }

    public IEnvironmentManager SetPreVariable(string key, string value)
    {
        setVariable(EnvironmentGroup.PreVariables, key, value);

        return this;
    }

    public IEnvironmentManager SetVariable(string key, string value)
    {
        setVariable(EnvironmentGroup.Variables, key, value);

        return this;
    }

    public IEnvironmentManager SetPreVariables(Dictionary<string, string> variables)
    {
        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(EnvironmentGroup.PreVariables, variable.Key, variable.Value);
        }

        return this;
    }

    public IEnvironmentManager SetVariables(Dictionary<string, string> variables)
    {
        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(EnvironmentGroup.Variables, variable.Key, variable.Value);
        }

        return this;
    }

    public IEnvironmentManager RemovePreVariable(string key)
    {
        removeVariable(EnvironmentGroup.PreVariables, key);

        return this;
    }

    public IEnvironmentManager RemoveVariable(string key)
    {
        removeVariable(EnvironmentGroup.Variables, key);

        return this;
    }

    public IEnvironmentManager RemovePreVariables(params string[] keys)
    {
        foreach (string key in keys)
        {
            removeVariable(EnvironmentGroup.PreVariables, key);
        }

        return this;
    }

    public IEnvironmentManager RemoveVariables(params string[] keys)
    {
        foreach (string key in keys)
        {
            removeVariable(EnvironmentGroup.Variables, key);
        }

        return this;
    }

    private bool tryAddFromFile(Dictionary<string, string> target, string filePath)
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
                    Debug.WriteLine($"Invalid environment variable format: {line}");

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

                setVariable(target, keyValue[0], keyValue[1]);
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

    private void setVariable(Dictionary<string, string> target, string key, string value)
    {
        switch (AdditionMode)
        {
            case EAddDataOption.SkipIfExists:
                if (target.ContainsKey(key))
                {
                    return;
                }

                break;
            case EAddDataOption.OverwriteIfExists:
            case EAddDataOption.OverwriteAll:
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
