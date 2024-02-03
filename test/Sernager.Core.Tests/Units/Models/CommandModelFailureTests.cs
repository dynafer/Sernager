using Sernager.Core.Models;
using System.Text.Json;

namespace Sernager.Core.Tests.Units.Models;

internal sealed class CommandModelFailureTests
{
    private readonly CommandModel mCommandModel = new CommandModel();
    [DatapointSource]
    private readonly object[] _ =
    [
        0,
        new int[0],
        'a',
        JsonDocument.Parse("{\"num\":1}").RootElement,
        JsonDocument.Parse("[1, 2, 3]").RootElement
    ];

    [Theory]
    public void CommandSetter_ShouldThrow_WhenInvalidTypeGiven(object testCase)
    {
        Assert.Throws<InvalidCastException>(() => mCommandModel.Command = testCase);
    }

    [Theory]
    public void ToCommandString_ShouldThrow_WhenInvalidTypeSet(object testCase)
    {
        PrivateUtil.SetFieldValue(mCommandModel, "mCommand", testCase);

        Assert.Throws<InvalidCastException>(() => mCommandModel.ToCommandString());
    }
}
