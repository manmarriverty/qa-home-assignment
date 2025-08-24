using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using CardValidation.Core.Services.Interfaces;
using CardValidation.ViewModels;
using CardValidation.Infrustructure;

namespace CardValidation.Tests.Unit.Infrustructure;

public class CreditCardValidationFilterTests
{
    private const string Required = "required";
    private const string Wrong = "wrong";
    
    private static ActionExecutingContext CreateContext(IDictionary<string, object?> arguments)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(),
            new ModelStateDictionary()
        );

        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), arguments, null!);
    }

    /*This is used to create an dummy CreditCard object for testing purposes only
    None of tests fail because of this object, since validations are mocked inside tests*/
    private static CreditCard CreateDummyCard()
    {
        return new CreditCard()
        {
            Owner = "noop",
            Date = "noop",
            Cvv = "noop",
            Number = "noop"
        };
    }

    [Fact]
    public void OnActionExecuting_AddsModelError_WhenDataIsEmpty()
    {
        // Arrange
        var cardValidationMock = new Mock<ICardValidationService>();
        cardValidationMock.Setup(s => s.ValidateOwner(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateIssueDate(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateNumber(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateCvc(It.IsAny<string>())).Returns(true);

        var filter = new CreditCardValidationFilter(cardValidationMock.Object);

        var card = new CreditCard()
        {
            Owner = "",
            Date = "",
            Cvv = "",
            Number = ""
        };

        var context = CreateContext(new Dictionary<string, object?>
        {
            { "creditCard", card }
        });

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.False(context.ModelState.IsValid);
        Assert.Contains(Required, context.ModelState[nameof(card.Owner)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Date)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Cvv)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Number)]!.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public void OnActionExecuting_AddsModelError_WhenDataIsNull()
    {
        // Arrange
        var cardValidationMock = new Mock<ICardValidationService>();
        cardValidationMock.Setup(s => s.ValidateOwner(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateIssueDate(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateNumber(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateCvc(It.IsAny<string>())).Returns(true);

        var filter = new CreditCardValidationFilter(cardValidationMock.Object);

        var card = new CreditCard()
        {
            Owner = null,
            Date = null,
            Cvv = null,
            Number = null
        };

        var context = CreateContext(new Dictionary<string, object?>
        {
            { "creditCard", card }
        });

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.False(context.ModelState.IsValid);
        Assert.Contains(Required, context.ModelState[nameof(card.Owner)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Date)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Cvv)]!.Errors[0].ErrorMessage);
        Assert.Contains(Required, context.ModelState[nameof(card.Number)]!.Errors[0].ErrorMessage);
    }

    [Fact]
    public void OnActionExecuting_AddsModelError_WhenCvvIsInvalid()
    {
        // Arrange
        var cardValidationMock = new Mock<ICardValidationService>();
        cardValidationMock.Setup(s => s.ValidateOwner(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateIssueDate(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateNumber(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateCvc(It.IsAny<string>())).Returns(false);

        var filter = new CreditCardValidationFilter(cardValidationMock.Object);

        var card = CreateDummyCard();

        var context = CreateContext(new Dictionary<string, object?>
        {
            { "creditCard", card }
        });

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.False(context.ModelState.IsValid);
        Assert.True(context.ModelState.ContainsKey(nameof(card.Cvv)));
        Assert.Contains(Wrong, context.ModelState[nameof(card.Cvv)]!.Errors[0].ErrorMessage.ToLowerInvariant());
        Assert.False(context.ModelState.ContainsKey(nameof(card.Owner)));
        Assert.False(context.ModelState.ContainsKey(nameof(card.Date)));
        Assert.False(context.ModelState.ContainsKey(nameof(card.Number)));
    }

    [Fact]
    public void OnActionExecuting_DoesNotAddModelError_WhenAllFieldsValid()
    {
        // Arrange
        var cardValidationMock = new Mock<ICardValidationService>();
        cardValidationMock.Setup(s => s.ValidateOwner(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateIssueDate(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateNumber(It.IsAny<string>())).Returns(true);
        cardValidationMock.Setup(s => s.ValidateCvc(It.IsAny<string>())).Returns(true);

        var filter = new CreditCardValidationFilter(cardValidationMock.Object);

        var card = CreateDummyCard();

        var context = CreateContext(new Dictionary<string, object?>
        {
            { "creditCard", card }
        });

        // Act
        filter.OnActionExecuting(context);

        // Assert
        Assert.True(context.ModelState.IsValid);
    }

    [Fact]
    public void OnActionExecuting_ThrowsException_WhenCreditCardArgumentIsNull()
    {
        // Arrange
        var cardValidationMock = new Mock<ICardValidationService>();
        var filter = new CreditCardValidationFilter(cardValidationMock.Object);

        var context = CreateContext(new Dictionary<string, object?>
        {
            { "creditCard", null }
        });

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => filter.OnActionExecuting(context));
    }
}
