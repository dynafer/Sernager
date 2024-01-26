using Sernager.Core.Configs;
using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Diagnostics;
using System.Text;

namespace Sernager.Core.Managers;

internal sealed class EnvironmentManager : IEnvironmentManager
{
    public EnvironmentModel Group { get; private set; }
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

        Group = Configurator.Config.EnvironmentGroups[name];
    }

    public void RemoveGroup()
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return;
        }

        Configurator.Config.EnvironmentGroups.Remove(Group.Name);
        foreach (CommandModel commandModel in Configurator.Config.Commands.Values)
        {
            commandModel.UsedEnvironmentGroups.Remove(Group.Name);
        }

        Group = null!;
    }

    public IEnvironmentManager AddFromPreFile(string filePath)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        if (AdditionMode == EAddDataOption.OverwriteAll)
        {
            Group.PreVariables.Clear();
        }

        tryAddFromFile(Group.PreVariables, filePath);

        return this;
    }

    public IEnvironmentManager AddFromFile(string filePath)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        if (AdditionMode == EAddDataOption.OverwriteAll)
        {
            Group.Variables.Clear();
        }

        tryAddFromFile(Group.Variables, filePath);

        return this;
    }

    public IEnvironmentManager AddPreLines(params string[] lines)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string line in lines)
        {
            tryAddLine(Group.PreVariables, line);
        }

        return this;
    }

    public IEnvironmentManager AddLines(params string[] lines)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string line in lines)
        {
            tryAddLine(Group.Variables, line);
        }

        return this;
    }

    public string? GetPreVariableOrNull(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return null;
        }

        return getVariableOrNull(Group.PreVariables, key);
    }

    public string? GetVariableOrNull(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return null;
        }

        return getVariableOrNull(Group.Variables, key);
    }

    public IEnvironmentManager SetPreVariable(string key, string value)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        setVariable(Group.PreVariables, key, value);

        return this;
    }

    public IEnvironmentManager SetVariable(string key, string value)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        setVariable(Group.Variables, key, value);

        return this;
    }

    public IEnvironmentManager SetPreVariables(Dictionary<string, string> variables)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(Group.PreVariables, variable.Key, variable.Value);
        }

        return this;
    }

    public IEnvironmentManager SetVariables(Dictionary<string, string> variables)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (KeyValuePair<string, string> variable in variables)
        {
            setVariable(Group.Variables, variable.Key, variable.Value);
        }

        return this;
    }

    public IEnvironmentManager RemovePreVariable(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        removeVariable(Group.PreVariables, key);

        return this;
    }

    public IEnvironmentManager RemoveVariable(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        removeVariable(Group.Variables, key);

        return this;
    }

    public IEnvironmentManager RemovePreVariables(params string[] keys)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string key in keys)
        {
            removeVariable(Group.PreVariables, key);
        }

        return this;
    }

    public IEnvironmentManager RemoveVariables(params string[] keys)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string key in keys)
        {
            removeVariable(Group.Variables, key);
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
                tryAddLine(target, line);
            }
        }

        Debug.WriteLine($"Env file loaded: {filePath}");

        return true;
    }

    private bool tryAddLine(Dictionary<string, string> target, string line)
    {
        line = line.Trim();
        if (line.StartsWith('#'))
        {
            return false;
        }

        StringBuilder keyStringBuilder = new StringBuilder();
        StringBuilder valueStringBuilder = new StringBuilder();

        bool bValue = false;
        bool bSingleQuote = false;
        bool bDoubleQuote = false;

        foreach (char c in line)
        {
            if (!bValue)
            {
                if (c == '#')
                {
                    break;
                }
                else if (c == ' ' || c == '\t')
                {
                    continue;
                }
                else if (c == '=')
                {
                    bValue = true;
                }
                else
                {
                    keyStringBuilder.Append(c);
                }

                continue;
            }

            if (valueStringBuilder.Length == 0)
            {
                if (c == ' ' || c == '\t')
                {
                    continue;
                }
                else if (c == '\'' || c == '"')
                {
                    bSingleQuote = c == '\'';
                    bDoubleQuote = c == '"';
                }
                else
                {
                    valueStringBuilder.Append(c);
                }

                continue;
            }

            if ((c == '\'' && bSingleQuote) ||
                (c == '"' && bDoubleQuote) ||
                (c == '#' && !bSingleQuote && !bDoubleQuote)
            )
            {
                bSingleQuote = false;
                bDoubleQuote = false;
                break;
            }
            else
            {
                valueStringBuilder.Append(c);
            }
        }

        if (!bValue ||
            bSingleQuote ||
            bDoubleQuote ||
            valueStringBuilder[valueStringBuilder.Length - 1] == '\'' ||
            valueStringBuilder[valueStringBuilder.Length - 1] == '"')
        {
            Debug.WriteLine($"Invalid environment variable format: {line}");
            return false;
        }

        setVariable(target, keyStringBuilder.ToString(), valueStringBuilder.ToString());
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
