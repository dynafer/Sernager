using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Builders;

public interface ISettingBuilder
{
    ISettingBuilder AddSettingName(string name, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingBuilder AddSettingNames(params string[] names);
    ISettingBuilder AddSettingNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingBuilder RemoveSettingName(string name);
    ISettingBuilder RemoveSettingNames(params string[] names);
    ISettingManager UseManager(string name);
}
