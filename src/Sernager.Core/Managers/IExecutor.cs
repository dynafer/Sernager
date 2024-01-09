using System.Diagnostics;

namespace Sernager.Core.Managers;

public interface IExecutor
{
    int Execute(DataReceivedEventHandler? outputHandler = null, DataReceivedEventHandler? errorHandler = null);
}
