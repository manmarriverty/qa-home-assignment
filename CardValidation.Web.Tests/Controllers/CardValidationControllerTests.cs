using CardValidation.Controllers;
using CardValidation.Core.Enums;
using CardValidation.Core.Services.Interfaces;
using CardValidation.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CardValidation.Web.Tests.Controllers
{
    public class CardValidationControllerTests
    {
        private readonly Mock<ICardValidationService> _cardValidationServiceMock;
        private readonly CardValidationController _sut;

        public CardValidationControllerTests()
        {
            _cardValidationServiceMock = new Mock<ICardValidationService>();
            _sut = new CardValidationController(_cardValidationServiceMock.Object);
        }

        [Fact]
        public void ValidateCreditCard_WhenModelStateIsInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            _sut.ModelState.AddModelError("Error", "Test error");
            var creditCard = new CreditCard();

            // Act
            var result = _sut.ValidateCreditCard(creditCard);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ValidateCreditCard_WhenCardIsValid_ShouldReturnOkWithPaymentType()
        {
            // Arrange
            var creditCard = new CreditCard { Number = "4111111111111111" };
            _cardValidationServiceMock
                .Setup(x => x.GetPaymentSystemType(It.IsAny<string>()))
                .Returns(PaymentSystemType.Visa);

            // Act
            var result = _sut.ValidateCreditCard(creditCard);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(PaymentSystemType.Visa);
        }
    }
}