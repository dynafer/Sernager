using Sernager.Core.Options;
using System.Diagnostics;

namespace Sernager.Core.Managers;

public static class ExceptionManager
{
    public static EErrorLevel ErrorLevel { get; set; } = EErrorLevel.None;

    [StackTraceHidden]
    public static void Throw<T>(string message, params object?[] args)
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
    public static void ThrowFail<T>(string message, params object?[] args)
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

        T? exception;

        try
        {
            exception = (T?)Activator.CreateInstance(typeof(T), combinedArgs);

            if (exception == null)
            {
                throw new Exception();
            }
        }
        catch
        {
            throw new NullReferenceException("Exception could not be created.");
        }

        return exception;
    }
}
