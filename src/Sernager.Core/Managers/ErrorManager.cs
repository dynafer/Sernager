using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Managers;

internal static class ErrorManager
{
    internal static EErrorLevel ErrorLevel { get; set; } = EErrorLevel.None;

    [StackTraceHidden]
    internal static void Throw<T>(string message, params object?[] args)
        where T : Exception
    {
        if (ErrorLevel == EErrorLevel.None)
        {
            return;
        }

        T exception = createException<T>(message, args);

        switch (ErrorLevel)
        {
            case EErrorLevel.Debug:
                Debug.WriteLine(exception);
                break;
            case EErrorLevel.Exception:
                throw exception;
        }
    }

    [StackTraceHidden]
    internal static void ThrowFail<T>(string message, params object?[] args)
        where T : Exception
    {
        if (ErrorLevel == EErrorLevel.None)
        {
            return;
        }

        T exception = createException<T>(message, args);

        switch (ErrorLevel)
        {
            case EErrorLevel.None:
                break;
            case EErrorLevel.Debug:
                Debug.Fail(exception.ToString());
                break;
            case EErrorLevel.Exception:
                throw exception;
        }
    }

    [StackTraceHidden]
    private static T createException<T>(string message, params object?[] args)
        where T : Exception
    {
        object?[] combinedArgs = new object?[args.Length + 1];
        combinedArgs[0] = message;
        args.CopyTo(combinedArgs, 1);

        T? exception = (T?)Activator.CreateInstance(typeof(T), combinedArgs);

        if (exception == null)
        {
            throw new NullReferenceException("Exception type must be inherited from System.Exception");
        }

        return exception;
    }
}
