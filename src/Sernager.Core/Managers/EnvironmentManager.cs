using Sernager.Core.Configs;
using Sernager.Core.Extensions;
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

    public IEnvironmentManager AddFromSubstFile(string filePath)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        if (AdditionMode == EAddDataOption.OverwriteAll)
        {
            Group.SubstVariables.Clear();
        }

        tryAddFromFile(Group.SubstVariables, filePath);

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

    public IEnvironmentManager AddSubstLines(params string[] lines)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string line in lines)
        {
            tryAddLine(Group.SubstVariables, line);
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

    public string? GetSubstVariableOrNull(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return null;
        }

        return getVariableOrNull(Group.SubstVariables, key);
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

    public IEnvironmentManager SetSubstVariable(string key, string value)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        setVariable(Group.SubstVariables, key, value);

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

    public IEnvironmentManager SetSubstVariables(Dictionary<string, string> variables)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (var variable in variables)
        {
            setVariable(Group.SubstVariables, variable.Key, variable.Value);
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

        foreach (var variable in variables)
        {
            setVariable(Group.Variables, variable.Key, variable.Value);
        }

        return this;
    }

    public IEnvironmentManager RemoveSubstVariable(string key)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        removeVariable(Group.SubstVariables, key);

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

    public IEnvironmentManager RemoveSubstVariables(params string[] keys)
    {
        if (Group == null)
        {
            ExceptionManager.Throw<SernagerException>("The group already removed.");
            return this;
        }

        foreach (string key in keys)
        {
            removeVariable(Group.SubstVariables, key);
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

        StringBuilder keyBuilder = new StringBuilder();
        StringBuilder valueBuilder = new StringBuilder();

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
                else if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
                {
                    continue;
                }
                else if (c == '=')
                {
                    bValue = true;
                }
                else
                {
                    keyBuilder.Append(c);
                }

                continue;
            }

            if (valueBuilder.Length == 0)
            {
                if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
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
                    valueBuilder.Append(c);
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
                valueBuilder.Append(c);
            }
        }

        if (!bValue ||
            bSingleQuote ||
            bDoubleQuote ||
            valueBuilder[valueBuilder.Length - 1] == '\'' ||
            valueBuilder[valueBuilder.Length - 1] == '"')
        {
            Debug.WriteLine($"Invalid environment variable format: {line}");
            return false;
        }

        setVariable(target, keyBuilder.ToString(), valueBuilder.ToString());
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

        Group.RemoveWhitespacesInDeclaredVariables(target, key);
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
