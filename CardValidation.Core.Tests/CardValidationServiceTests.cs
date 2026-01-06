using CardValidation.Core.Enums;
using CardValidation.Core.Services;
using Xunit;

namespace CardValidation.Core.Tests
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService _service;

        public CardValidationServiceTests()
        {
            _service = new CardValidationService();
        }

        #region ValidateNumber Tests

        [Theory]
        [InlineData("4111111111111111", true)]
        [InlineData("5555555555554444", true)]
        [InlineData("378282246310005", true)]
        [InlineData("1234567890123456", false)]
        public void ValidateNumber_ValidatesCardNumber_ReturnsExpectedResult(string cardNumber, bool expectedResult)
        {
            // Act
            var result = _service.ValidateNumber(cardNumber);

            // Assert
            Assert.Equal(expectedResult, result);
        }


        #endregion

        #region GetPaymentSystemType Tests

        [Theory]
        [InlineData("4111111111111111", PaymentSystemType.Visa)]
        [InlineData("5555555555554444", PaymentSystemType.MasterCard)]
        [InlineData("378282246310005", PaymentSystemType.AmericanExpress)]
        public void GetPaymentSystemType_ValidCardNumber_ReturnsExpectedType(string cardNumber, PaymentSystemType expectedType)
        {
            // Act
            var result = _service.GetPaymentSystemType(cardNumber);

            // Assert
            Assert.Equal(expectedType, result);
        }

        [Fact]
        public void GetPaymentSystemType_InvalidCardNumber_ThrowsNotImplementedException()
        {
            // Arrange
            var cardNumber = "1234567890123456";

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => _service.GetPaymentSystemType(cardNumber));
        }

        #endregion

        #region ValidateOwner Tests

        [Fact]
        public void ValidateOwner_ValidName_ReturnsTrue()
        {
            // Arrange
            var owner = "John Doe";

            // Act
            var result = _service.ValidateOwner(owner);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateOwner_ValidNameWithThreeWords_ReturnsTrue()
        {
            // Arrange
            var owner = "John Michael Doe";

            // Act
            var result = _service.ValidateOwner(owner);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateOwner_InvalidNameWithNumbers_ReturnsFalse()
        {
            // Arrange
            var owner = "John123 Doe";

            // Act
            var result = _service.ValidateOwner(owner);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOwner_NameWithCardNumber_ReturnsFalse()
        {
            // Arrange
            var owner = "John Doe 4111111111111111"; // Name contains card number

            // Act
            var result = _service.ValidateOwner(owner);

            // Assert
            Assert.False(result);
        }


        #endregion

        #region ValidateIssueDate Tests

        [Fact]
        public void ValidateIssueDate_ValidDate_ReturnsTrue()
        {
            // Arrange
            // Using future date to work with current implementation logic
            var futureYear = DateTime.UtcNow.Year + 2;
            var issueDate = $"12/{futureYear}";

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateIssueDate_ValidDateWithSlash_ReturnsTrue()
        {
            // Arrange
            var futureYear = DateTime.UtcNow.Year + 2;
            var issueDate = $"01/{futureYear}";

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateIssueDate_ValidDateWithoutSlash_ReturnsTrue()
        {
            // Arrange
            var futureYear = DateTime.UtcNow.Year + 2;
            var issueDate = $"12{futureYear}";

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateIssueDate_InvalidFormat_ReturnsFalse()
        {
            // Arrange
            var issueDate = "13/2025"; // Invalid month

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateIssueDate_ExpiredCard_ReturnsFalse()
        {
            // Arrange
            var issueDate = "01/2020"; // Expired card

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateIssueDate_CurrentYearDate_ReturnsTrue()
        {
            // Arrange
            var currentYear = DateTime.UtcNow.Year;
            var issueDate = $"12/{currentYear}";

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public void ValidateIssueDate_TwoDigitYear99_ReturnsTrue()
        {
            // Arrange
            var issueDate = "12/99"; // Year 99 should be interpreted as 2099

            // Act
            var result = _service.ValidateIssueDate(issueDate);

            // Assert
            Assert.True(result);
        }


        #endregion

        #region ValidateCvc Tests

        [Theory]
        [InlineData("123", true)]
        [InlineData("1234", true)]
        [InlineData("12", false)]
        [InlineData("12345", false)]
        public void ValidateCvc_ValidatesCvcFormat_ReturnsExpectedResult(string cvc, bool expectedResult)
        {
            // Act
            var result = _service.ValidateCvc(cvc);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ValidateCvc_ValidatesFormat_ReturnsTrue()
        {
            // Arrange
            var visaCardNumber = "4111111111111111";
            var cvc = "123";

            // Act
            var isValidCard = _service.ValidateNumber(visaCardNumber);
            var isValidCvc = _service.ValidateCvc(cvc);

            // Assert
            Assert.True(isValidCard);
            Assert.True(isValidCvc);
        }

        #endregion
    }
}
