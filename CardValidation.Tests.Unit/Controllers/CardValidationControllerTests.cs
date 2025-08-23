using CardValidation.Controllers;
using CardValidation.Core.Enums;
using CardValidation.Core.Services.Interfaces;
using CardValidation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CardValidation.Tests.Unit.Controllers;

public class CardValidationControllerTests
{
    [Fact]
    public void ValidateCreditCard_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var serviceMock = new Mock<ICardValidationService>();
        var controller = new CardValidationController(serviceMock.Object);
        
        controller.ModelState.AddModelError("Number", "Number is required");

        // Ignore the CreditCard setup since we are testing model validation here
        var creditCard = new CreditCard
        {
            Number = "",
            Owner = "John Doe",
            Date = "12/2025",
            Cvv = "123"
        };

        // Act
        var result = controller.ValidateCreditCard(creditCard);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequest.Value);
        Assert.True(controller.ModelState.ContainsKey("Number"));
    }

    [Fact]
    public void ValidateCreditCard_ReturnsOk_WithPaymentSystemType_WhenModelIsValid()
    {
        // Arrange
        var serviceMock = new Mock<ICardValidationService>();
        serviceMock.Setup(s => s.GetPaymentSystemType(It.IsAny<string>())).Returns(PaymentSystemType.Visa);

        var controller = new CardValidationController(serviceMock.Object);

        var creditCard = new CreditCard
        {
            Number = "4111111111111111",
            Owner = "John Doe",
            Date = "12/2025",
            Cvv = "123"
        };

        // Act
        var result = controller.ValidateCreditCard(creditCard);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(PaymentSystemType.Visa, okResult.Value);
    }
}