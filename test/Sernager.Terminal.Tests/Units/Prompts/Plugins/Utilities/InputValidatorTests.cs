using Sernager.Terminal.Prompts.Plugins.Utilities;
using Sernager.Terminal.Tests.Fixtures;

namespace Sernager.Terminal.Tests.Units.Prompts.Plugins.Utilities;

internal sealed class InputValidatorTests : InputValidatorFixture
{
    [Theory]
    public void AddRule_ShouldAdd(EInputValidatorHandlerType handlerType)
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRule(TrueHandler, null, handlerType);
        inputValidator.AddRule(FalseHandler, "Error", handlerType);
    }

    [Test]
    public void AddRules_ShouldAdd_WithOnlyHandlers()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules(TrueHandler, FalseHandler, TrueHandler);
    }

    [Test]
    public void AddRules_ShouldAdd_WithHandlersAndErrorMessagesOrNull()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules((FalseHandler, "Error 1"), (TrueHandler, null), (FalseHandler, "Error 3"));
    }

    [Test]
    public void AddRules_ShouldAdd_WithHandlersAndHandlerTypes()
    {
        (InputValidationHandler, EInputValidatorHandlerType)[] parameters =
        [
            (TrueHandler, EInputValidatorHandlerType.Default),
            (TrueHandler, EInputValidatorHandlerType.ReturnWhenTrue),
            (FalseHandler, EInputValidatorHandlerType.Default),
        ];

        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules(parameters);
    }

    [Test]
    public void AddRules_ShouldAdd_WithAllData()
    {
        (InputValidationHandler, string?, EInputValidatorHandlerType)[] parameters =
        [
            (FalseHandler, "Error 1", EInputValidatorHandlerType.Default),
            (TrueHandler, null, EInputValidatorHandlerType.ReturnWhenTrue),
            (TrueHandler, null, EInputValidatorHandlerType.Default),
        ];

        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules(parameters);
    }

    [Test]
    public void Validate_ShouldReturnFalse()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules(TrueHandler, FalseHandler, FalseHandler);

        bool result = inputValidator.Validate("input");

        Assert.That(result, Is.False);
        Assert.That(inputValidator.ErrorMessage, Is.Null);
        Assert.That(Count, Is.EqualTo(2));
    }

    [Test]
    public void Validate_ShouldReturnFalseAndSetErrorMessage()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules((TrueHandler, null), (FalseHandler, "Error 2"), (FalseHandler, "Error 3"));

        bool result = inputValidator.Validate("input");

        Assert.That(result, Is.False);
        Assert.That(inputValidator.ErrorMessage, Is.EqualTo("Error 2"));
        Assert.That(Count, Is.EqualTo(2));
    }

    [Test]
    public void Validate_ShouldReturnTrueAndSkip_WhenHandlerTypeIsReturnWhenTrueAndResultIsFalse()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules((TrueHandler, EInputValidatorHandlerType.Default), (FalseHandler, EInputValidatorHandlerType.ReturnWhenTrue), (TrueHandler, EInputValidatorHandlerType.Default));

        bool result = inputValidator.Validate("input");

        Assert.That(result, Is.True);
        Assert.That(Count, Is.EqualTo(3));
    }

    [Test]
    public void Validate_ShouldReturnTrue_WhenHandlerTypeIsReturnWhenTrue()
    {
        InputValidator inputValidator = new InputValidator();

        inputValidator.AddRules((TrueHandler, EInputValidatorHandlerType.Default), (TrueHandler, EInputValidatorHandlerType.ReturnWhenTrue), (TrueHandler, EInputValidatorHandlerType.Default));

        bool result = inputValidator.Validate("input");

        Assert.That(result, Is.True);
        Assert.That(Count, Is.EqualTo(2));
    }
}
