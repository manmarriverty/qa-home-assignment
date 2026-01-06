using CardValidation.Core.Services;
using CardValidation.Core.Services.Interfaces;
using CardValidation.Infrustructure;
using CardValidation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace CardValidation.Core.Tests
{
    public class CreditCardValidationFilterTests
    {
        private readonly ICardValidationService _validationService;
        private readonly CreditCardValidationFilter _filter;

        public CreditCardValidationFilterTests()
        {
            _validationService = new CardValidationService();
            _filter = new CreditCardValidationFilter(_validationService);
        }

        [Fact]
        public void OnActionExecuting_ValidCreditCard_NoModelStateErrors()
        {
            // Arrange
            var futureYear = DateTime.UtcNow.Year + 2;
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = $"12/{futureYear}",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.True(context.ModelState.IsValid);
        }

        [Fact]
        public void OnActionExecuting_MissingOwner_AddsModelStateError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = null,
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Owner"));
        }

        [Fact]
        public void OnActionExecuting_MissingNumber_AddsModelStateError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = null,
                Date = "12/2025",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Number"));
        }

        [Fact]
        public void OnActionExecuting_InvalidOwner_AddsWrongParameterError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John123", // Invalid - contains numbers
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Owner"));
        }

        [Fact]
        public void OnActionExecuting_InvalidCardNumber_AddsWrongParameterError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "1234567890123456", // Invalid card number
                Date = "12/2025",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Number"));
        }

        [Fact]
        public void OnActionExecuting_InvalidDate_AddsWrongParameterError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = "13/2025", // Invalid month
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Date"));
        }

        [Fact]
        public void OnActionExecuting_InvalidCvc_AddsWrongParameterError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "12" // Invalid - too short
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Cvv"));
        }

        [Fact]
        public void OnActionExecuting_NullCreditCard_ThrowsInvalidOperationException()
        {
            // Arrange
            CreditCard? creditCard = null;

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _filter.OnActionExecuting(context));
        }

        [Fact]
        public void OnActionExecuting_EmptyStringOwner_AddsRequiredError()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "", // Empty string
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.False(context.ModelState.IsValid);
            Assert.True(context.ModelState.ContainsKey("Owner"));
        }

        [Fact]
        public void OnActionExecuting_ValidatesAllParameters()
        {
            // Arrange
            var mockService = new Mock<ICardValidationService>();
            mockService.Setup(s => s.ValidateOwner(It.IsAny<string>())).Returns(true);
            mockService.Setup(s => s.ValidateIssueDate(It.IsAny<string>())).Returns(true);
            mockService.Setup(s => s.ValidateCvc(It.IsAny<string>())).Returns(true);
            mockService.Setup(s => s.ValidateNumber(It.IsAny<string>())).Returns(true);

            var filter = new CreditCardValidationFilter(mockService.Object);

            var futureYear = DateTime.UtcNow.Year + 2;
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = $"12/{futureYear}",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            filter.OnActionExecuting(context);

            // Assert
            mockService.Verify(s => s.ValidateOwner(It.IsAny<string>()), Times.Once);
            mockService.Verify(s => s.ValidateNumber(It.IsAny<string>()), Times.Once);
            mockService.Verify(s => s.ValidateIssueDate(It.IsAny<string>()), Times.Once);
            mockService.Verify(s => s.ValidateCvc(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void OnActionExecuting_MultipleValidations_NoInterference()
        {
            // Arrange
            var futureYear = DateTime.UtcNow.Year + 2;
            var creditCard1 = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = $"12/{futureYear}",
                Cvv = "123"
            };

            var creditCard2 = new CreditCard
            {
                Owner = "Jane Smith",
                Number = "5555555555554444",
                Date = $"06/{futureYear}",
                Cvv = "456"
            };

            var actionContext1 = CreateActionContext(creditCard1);
            var context1 = new ActionExecutingContext(
                actionContext1,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard1 } },
                null!);

            var actionContext2 = CreateActionContext(creditCard2);
            var context2 = new ActionExecutingContext(
                actionContext2,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard2 } },
                null!);

            // Act
            _filter.OnActionExecuting(context1);
            _filter.OnActionExecuting(context2);

            // Assert
            Assert.True(context1.ModelState.IsValid);
            Assert.True(context2.ModelState.IsValid);
        }

        [Fact]
        public void OnActionExecuting_ValidatesAndSetsModelState()
        {
            // Arrange
            var futureYear = DateTime.UtcNow.Year + 2;
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = $"12/{futureYear}",
                Cvv = "123"
            };

            var actionContext = CreateActionContext(creditCard);
            var initialErrorCount = actionContext.ModelState.ErrorCount;

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                null!);

            // Act
            _filter.OnActionExecuting(context);

            // Assert
            Assert.True(context.ModelState.IsValid);
            Assert.Equal(initialErrorCount, context.ModelState.ErrorCount);
        }

        private ActionContext CreateActionContext(CreditCard? creditCard)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var modelState = new ModelStateDictionary();

            return new ActionContext(httpContext, routeData, actionDescriptor, modelState);
        }
    }
}
