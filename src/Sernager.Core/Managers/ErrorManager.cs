using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Managers;

internal static class ErrorManager
{
    internal static EErrorLevel ErrorLevel { get; set; } = EErrorLevel.None;

    [StackTraceHidden]
    internal static void Throw(Exception exception, bool bDebugFail = false)
    {
        switch (ErrorLevel)
        {
            case EErrorLevel.None:
                break;
            case EErrorLevel.Debug:
                if (bDebugFail)
                {
                    Debug.Fail(exception.ToString());
                }
                else
                {
                    Debug.WriteLine(exception);
                }

                break;
            case EErrorLevel.Exception:
                throw exception;
        }
    }
}
