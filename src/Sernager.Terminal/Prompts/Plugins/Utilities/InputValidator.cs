using TInputValidatorItem = (
    Sernager.Terminal.Prompts.Plugins.Utilities.InputValidationHandler,
    string?,
    Sernager.Terminal.Prompts.Plugins.Utilities.EInputValidatorHandlerType
);

namespace Sernager.Terminal.Prompts.Plugins.Utilities;

internal sealed class InputValidator
{
    private readonly List<TInputValidatorItem> mValidators = new List<TInputValidatorItem>();
    internal string? ErrorMessage { get; set; } = null;

    internal InputValidator AddRule(InputValidationHandler validator, string? errorMessage = null, EInputValidatorHandlerType type = EInputValidatorHandlerType.Default)
    {
        mValidators.Add((validator, errorMessage, type));

        return this;
    }

    internal InputValidator AddRules(params InputValidationHandler[] validators)
    {
        foreach (InputValidationHandler validator in validators)
        {
            AddRule(validator);
        }

        return this;
    }

    internal InputValidator AddRules(params (InputValidationHandler, string?)[] validators)
    {
        foreach ((InputValidationHandler validator, string? errorMessage) in validators)
        {
            AddRule(validator, errorMessage);
        }

        return this;
    }

    internal InputValidator AddRules(params (InputValidationHandler, EInputValidatorHandlerType)[] validators)
    {
        foreach ((InputValidationHandler validator, EInputValidatorHandlerType type) in validators)
        {
            AddRule(validator, type: type);
        }

        return this;
    }

    internal InputValidator AddRules(params TInputValidatorItem[] validators)
    {
        foreach (TInputValidatorItem validator in validators)
        {
            mValidators.Add(validator);
        }

        return this;
    }

    internal bool Validate(string input)
    {
        foreach ((InputValidationHandler validator, string? errorMessage, EInputValidatorHandlerType handlerType) in mValidators)
        {
            bool result = validator(input);

            switch (handlerType)
            {
                case EInputValidatorHandlerType.ReturnWhenTrue:
                    if (result)
                    {
                        return true;
                    }

                    break;
                default:
                    if (!result)
                    {
                        ErrorMessage = errorMessage;

                        return false;
                    }

                    break;
            }
        }

        return true;
    }
}