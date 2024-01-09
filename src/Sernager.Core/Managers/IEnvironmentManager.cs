using Sernager.Core.Models;
using Sernager.Core.Options;
using System.Collections.ObjectModel;

namespace Sernager.Core.Managers;

public interface IEnvironmentManager
{
    EnvironmentModel EnvironmentGroup { get; }
    EAddDataOption AdditionMode { get; set; }
    void RemoveGroup();
    IEnvironmentManager AddFromPreFile(string filePath);
    IEnvironmentManager AddFromFile(string filePath);
    IEnvironmentManager AddPreLines(params string[] lines);
    IEnvironmentManager AddLines(params string[] lines);
    string? GetPreVariableOrNull(string key);
    string? GetVariableOrNull(string key);
    ReadOnlyDictionary<string, string> GetPreVariables();
    ReadOnlyDictionary<string, string> GetVariables();
    IEnvironmentManager SetPreVariable(string key, string value);
    IEnvironmentManager SetVariable(string key, string value);
    IEnvironmentManager SetPreVariables(Dictionary<string, string> keyValues);
    IEnvironmentManager SetVariables(Dictionary<string, string> keyValues);
    IEnvironmentManager RemovePreVariable(string key);
    IEnvironmentManager RemoveVariable(string key);
    IEnvironmentManager RemovePreVariables(params string[] keys);
    IEnvironmentManager RemoveVariables(params string[] keys);
}
