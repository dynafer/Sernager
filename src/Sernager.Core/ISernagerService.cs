using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

public interface ISernagerService
{
    IEnvironmentManager ManageEnvironmentGroup(string groupName, string shortName = "", string description = "");
    ICommandManager ManageCommandGroup(string groupName, string shortName = "", string description = "");
    string[] GetCommandGroupNames();
    void SaveAs(EConfigurationType type = EConfigurationType.Sernager);
}
