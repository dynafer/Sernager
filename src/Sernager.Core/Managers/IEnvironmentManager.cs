using Sernager.Core.Models;
using Sernager.Core.Options;

namespace Sernager.Core.Managers;

public interface IEnvironmentManager
{
    EnvironmentModel Group { get; }
    EAddDataOption AdditionMode { get; set; }
    void RemoveGroup();
    IEnvironmentManager AddFromSubstFile(string filePath);
    IEnvironmentManager AddFromFile(string filePath);
    IEnvironmentManager AddSubstLines(params string[] lines);
    IEnvironmentManager AddLines(params string[] lines);
    string? GetSubstVariableOrNull(string key);
    string? GetVariableOrNull(string key);
    IEnvironmentManager SetSubstVariable(string key, string value);
    IEnvironmentManager SetVariable(string key, string value);
    IEnvironmentManager SetSubstVariables(Dictionary<string, string> keyValues);
    IEnvironmentManager SetVariables(Dictionary<string, string> keyValues);
    IEnvironmentManager RemoveSubstVariable(string key);
    IEnvironmentManager RemoveVariable(string key);
    IEnvironmentManager RemoveSubstVariables(params string[] keys);
    IEnvironmentManager RemoveVariables(params string[] keys);
}
