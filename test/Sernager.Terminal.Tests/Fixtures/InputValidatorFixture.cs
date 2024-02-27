using Sernager.Terminal.Prompts.Plugins.Utilities;

namespace Sernager.Terminal.Tests.Fixtures;

internal abstract class InputValidatorFixture
{
    internal int Count { get; private set; } = 0;
    internal InputValidationHandler TrueHandler
    {
        get
        {
            return (string input) =>
            {
                ++Count;
                return true;
            };
        }
    }

    internal InputValidationHandler FalseHandler
    {
        get
        {
            return (string input) =>
            {
                ++Count;
                return false;
            };
        }
    }

    [TearDown]
    public void ResetCount()
    {
        Count = 0;
    }
}
