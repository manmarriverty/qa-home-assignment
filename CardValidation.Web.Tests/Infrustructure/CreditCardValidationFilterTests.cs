using CardValidation.Core.Services.Interfaces;
using CardValidation.Infrustructure;
using CardValidation.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace CardValidation.Web.Tests.Infrastructure
{
    public class CreditCardValidationFilterTests
    {
        private readonly Mock<ICardValidationService> _cardValidationServiceMock;
        private readonly CreditCardValidationFilter _sut;

        public CreditCardValidationFilterTests()
        {
            _cardValidationServiceMock = new Mock<ICardValidationService>();
            _sut = new CreditCardValidationFilter(_cardValidationServiceMock.Object);
        }

        [Fact]
        public void OnActionExecuting_WhenAllParametersAreValid_ShouldNotAddModelErrors()
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = "John Doe",
                Date = "12/25",
                Cvv = "123",
                Number = "4111111111111111"
            };

            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            var modelState = new ModelStateDictionary();

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                new Mock<Controller>().Object);

            _cardValidationServiceMock.Setup(x => x.ValidateOwner(It.IsAny<string>())).Returns(true);
            _cardValidationServiceMock.Setup(x => x.ValidateIssueDate(It.IsAny<string>())).Returns(true);
            _cardValidationServiceMock.Setup(x => x.ValidateCvc(It.IsAny<string>())).Returns(true);
            _cardValidationServiceMock.Setup(x => x.ValidateNumber(It.IsAny<string>())).Returns(true);

            // Act
            _sut.OnActionExecuting(context);

            // Assert
            context.ModelState.ErrorCount.Should().Be(0);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void OnActionExecuting_WhenRequiredParametersAreMissing_ShouldAddModelErrors(string value)
        {
            // Arrange
            var creditCard = new CreditCard
            {
                Owner = value,
                Date = value,
                Cvv = value,
                Number = value
            };

            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            var modelState = new ModelStateDictionary();

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "creditCard", creditCard } },
                new Mock<Controller>().Object);

            // Act
            _sut.OnActionExecuting(context);

            // Assert
            context.ModelState.ErrorCount.Should().Be(4);
            context.ModelState.Keys.Should().Contain(new[] { "Owner", "Date", "Cvv", "Number" });
        }
    }
}