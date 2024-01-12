using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core;

public interface ISernagerService
{
    ICommandManager ManageCommandGroup(string groupName, string shortName = "", string description = "");
    IEnvironmentManager ManageEnvironmentGroup(string groupName);
    IExecutor GetExecutor(Guid commandId);
    string[] GetCommandGroupNames();
    string[] GetCommandGroupShortNames();
    string[] GetEnvironmentGroupNames();
    void SaveAs(EConfigurationType type);
    void SaveAs(EUserFriendlyConfigurationType type);
}
