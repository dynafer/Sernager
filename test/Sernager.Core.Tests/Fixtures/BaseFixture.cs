using Sernager.Core.Managers;
using Sernager.Core.Options;

namespace Sernager.Core.Tests.Fixtures;

public class BaseFixture
{
    public BaseFixture()
    {
        ErrorManager.ErrorLevel = EErrorLevel.Exception;
    }
}
