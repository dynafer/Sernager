using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Builders;

public interface ISettingBuilder
{
    ISettingBuilder AddName(string name, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingBuilder AddNames(params string[] names);
    ISettingBuilder AddNames(IEnumerable<string> names, EAddDataOption option = EAddDataOption.SkipIfExists);
    ISettingBuilder RemoveName(string name);
    ISettingBuilder RemoveNames(params string[] names);
    ISettingManager UseManager(string name);
}
