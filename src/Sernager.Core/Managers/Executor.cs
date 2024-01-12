using Sernager.Core.Models;
using System.Diagnostics;

namespace Sernager.Core.Managers;

internal sealed class Executor : IExecutor
{
    private readonly CommandModel mModel;

    internal Executor(CommandModel model)
    {
        mModel = model;
    }

    public int Execute(DataReceivedEventHandler? outputHandler = null, DataReceivedEventHandler? errorHandler = null)
    {
        using (Process process = createProcess())
        {
            if (outputHandler != null)
            {
                process.OutputDataReceived += outputHandler;
            }

            if (errorHandler != null)
            {
                process.ErrorDataReceived += errorHandler;
            }

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            int exitCode = process.ExitCode;

            return exitCode;
        }
    }

    private Process createProcess()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        switch (mModel.Command)
        {
            case string[] commandArray:
                startInfo.FileName = commandArray[0];
                for (int i = 1; i < commandArray.Length; ++i)
                {
                    startInfo.ArgumentList.Add(commandArray[i]);
                }

                break;
            case string commandString:
                startInfo.FileName = commandString.Split(' ')[0];
                if (commandString.Length > startInfo.FileName.Length + 1)
                {
                    startInfo.Arguments = commandString.Substring(startInfo.FileName.Length + 1);
                }

                break;
            default:
                throw new InvalidCastException("Command must be a string or string[].");
        }

        return new Process()
        {
            StartInfo = startInfo
        };
    }
}
