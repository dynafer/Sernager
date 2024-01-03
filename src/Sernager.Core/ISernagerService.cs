using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

public interface ISernagerService
{
    ISettingManager ManageSetting(string settingName);
    IGroupManager ManageGroup(string groupName);
    string[] GetGroupNames();
    void SaveAs(EConfigurationType type = EConfigurationType.Sernager);
}
