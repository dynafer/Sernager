using Sernager.Core.Managers;

namespace Sernager.Core;

public interface ISernagerService
{
    ISettingManager ManageSetting(string settingName);
}
