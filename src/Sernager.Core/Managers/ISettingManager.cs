using Sernager.Core.Options;
using System.Collections.ObjectModel;

namespace Sernager.Core.Managers;

public interface ISettingManager
{
    void RemoveSetting();
    ISettingManager AddEnvFile(string filePath, EAddDataOption option = EAddDataOption.SkipIfExists);
    string? GetEnvVariableOrNull(string key);
    ReadOnlyDictionary<string, string> GetEnvVariables();
    ISettingManager SetEnvVariable(string key, string value, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingManager SetEnvVariables(IDictionary<string, string> keyValues, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingManager RemoveEnvVariable(string key);
    ISettingManager RemoveEnvVariables(params string[] keys);
}
