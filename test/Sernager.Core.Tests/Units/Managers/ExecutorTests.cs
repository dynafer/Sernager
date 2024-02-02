using Sernager.Core.Configs;
using Sernager.Core.Managers;
using Sernager.Core.Models;
using System.Diagnostics;
using System.Text;

namespace Sernager.Core.Tests.Units.Managers;

public class ExecutorTests
{
    [DatapointSource]
    private static readonly string[] COMMANDS =
    [
        $"{CaseUtil.GetPath("Commands.Failure", "cmd")}",
        $"[Array]{CaseUtil.GetPath("Commands.Failure", "cmd")}",
        $"{CaseUtil.GetPath("Commands.Success", "cmd")}",
        $"[Array]{CaseUtil.GetPath("Commands.Success", "cmd")}",
    ];
    private static readonly string EXPECTED_SUCCESS_CASE = @"Test environment variable
Substitution will be applied: Substitution applied.
Executed.";
    private static readonly string EXPECTED_FAILURE_CASE = "Env variable doesn't exist:";
    private CommandModel mModel;
    private Executor mExecutor;

    [SetUp]
    public void SetUp()
    {
        Configurator.Init();
        EnvironmentModel environmentModel = new EnvironmentModel()
        {
            Name = "Test",
            SubstVariables = new Dictionary<string, string>()
            {
                { "SUBST_ENV", "Substitution applied." }
            },
            Variables = new Dictionary<string, string>()
            {
                { "ENV1", "Test environment variable" },
                { "ENV2", "Substitution will be applied: ${SUBST_ENV}"},
                { "ENV3", "Executed." }
            }
        };

        Configurator.Config.EnvironmentGroups.Add("Test", environmentModel);

        mModel = new CommandModel()
        {
            Name = "Test",
            ShortName = "t",
            UsedEnvironmentGroups = new List<string>() { "Test" },
        };

        mExecutor = new Executor(mModel);
    }

    [TearDown]
    public void Reset()
    {
        ResetUtil.ResetConfigurator();
    }

    [Theory]
    public void Execute_ShouldReturnExitCode(string command)
    {
        Assume.That(command, Is.AnyOf(COMMANDS));

        bool bSuccess = isSuccessCaseAndSetCommand(command);
        int exitCode = mExecutor.Execute();

        Assert.That(exitCode, Is.EqualTo(bSuccess ? 0 : 1));
    }

    [Theory]
    public void Execute_ShouldHandleOutput(string command)
    {
        Assume.That(command, Is.AnyOf(COMMANDS));

        bool bSuccess = isSuccessCaseAndSetCommand(command);

        StringBuilder outputBuilder = new StringBuilder();
        StringBuilder errorBuilder = new StringBuilder();

        DataReceivedEventHandler outputHandler = (sender, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        DataReceivedEventHandler errorHandler = (sender, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        int exitCode = mExecutor.Execute(outputHandler, errorHandler);
        string output = outputBuilder.ToString().Trim();
        string error = errorBuilder.ToString().Trim();

        Assert.That(exitCode, Is.EqualTo(bSuccess ? 0 : 1));

        if (bSuccess)
        {
            Assert.That(output, Is.EqualTo(EXPECTED_SUCCESS_CASE));
            Assert.That(error, Is.Empty);
        }
        else
        {
            Assert.That(output, Is.Empty.Or.EqualTo(EXPECTED_FAILURE_CASE));
            Assert.That(error, Is.Empty.Or.EqualTo(EXPECTED_FAILURE_CASE));
        }
    }

    [Theory]
    public void Execute_ShouldHandleOutput_WithArguments(string command)
    {
        Assume.That(command, Is.AnyOf(COMMANDS));

        string[] arguments = ["arg1", "arg2"];

        bool bSuccess = isSuccessCaseAndSetCommand($"{command} {string.Join(' ', arguments)}");

        StringBuilder outputBuilder = new StringBuilder();
        StringBuilder errorBuilder = new StringBuilder();

        DataReceivedEventHandler outputHandler = (sender, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        DataReceivedEventHandler errorHandler = (sender, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        int exitCode = mExecutor.Execute(outputHandler, errorHandler);
        string output = outputBuilder.ToString().Trim();
        string error = errorBuilder.ToString().Trim();

        Assert.That(exitCode, Is.EqualTo(bSuccess ? 0 : 1));

        if (bSuccess)
        {
            string expectedOutput = $"{EXPECTED_SUCCESS_CASE}\r\nArguments: {string.Join(' ', arguments)}";
            Assert.That(output, Is.EqualTo(expectedOutput));
            Assert.That(error, Is.Empty);
        }
        else
        {
            string expectedOutput = $"Arguments: {string.Join(' ', arguments)}\r\n{EXPECTED_FAILURE_CASE}";
            Assert.That(output, Is.Empty.Or.EqualTo(expectedOutput));
            Assert.That(error, Is.Empty.Or.EqualTo(expectedOutput));
        }
    }

    private bool isSuccessCaseAndSetCommand(string command)
    {
        if (command.Contains("[Array]"))
        {
            command = command.Replace("[Array]", string.Empty);
            mModel.Command = command.Split(' ');
        }
        else
        {
            mModel.Command = command;
        }

        return command.Contains("Success");
    }
}
