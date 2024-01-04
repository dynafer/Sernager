using Sernager.Core.Options;
using System.Collections.ObjectModel;

namespace Sernager.Core.Managers;

public interface IEnvironmentManager
{
    void RemoveGroup();
    IEnvironmentManager AddPreFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists);
    IEnvironmentManager AddFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists);
    string? GetPreVariableOrNull(string key);
    string? GetVariableOrNull(string key);
    ReadOnlyDictionary<string, string> GetPreVariables();
    ReadOnlyDictionary<string, string> GetVariables();
    IEnvironmentManager SetPreVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists);
    IEnvironmentManager SetVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists);
    IEnvironmentManager SetPreVariables(Dictionary<string, string> keyValues, EAddDataOption option = EAddDataOption.SkipIfExists);
    IEnvironmentManager SetVariables(Dictionary<string, string> keyValues, EAddDataOption option = EAddDataOption.SkipIfExists);
    IEnvironmentManager RemovePreVariable(string key);
    IEnvironmentManager RemoveVariable(string key);
    IEnvironmentManager RemovePreVariables(params string[] keys);
    IEnvironmentManager RemoveVariables(params string[] keys);
}
