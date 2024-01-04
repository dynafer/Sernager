using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

public interface ISernagerService
{
    ICommandManager ManageCommandGroup(string groupName, string shortName = "", string description = "");
    IEnvironmentManager ManageEnvironmentGroup(string groupName);
    string[] GetCommandGroupNames();
    string[] GetEnvironmentGroupNames();
    void SaveAs(EConfigurationType type = EConfigurationType.Sernager);
}
