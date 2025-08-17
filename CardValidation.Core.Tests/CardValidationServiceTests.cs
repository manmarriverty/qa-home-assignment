using Xunit;
using CardValidation.Core.Services;
using CardValidation.Core.Enums;

namespace CardValidation.Core.Tests
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService _service = new CardValidationService();

        // ====================
        // Individual Validations
        // ====================

        [Theory]
        [InlineData("John Doe", true)]
        [InlineData("Jane", true)]
        [InlineData("John123", false)]
        [InlineData("", false)]
        public void ValidateOwner_ShouldWorkCorrectly(string owner, bool expected)
        {
            Assert.Equal(expected, _service.ValidateOwner(owner));
        }

        [Theory]
        [InlineData("4111111111111111", true)]   // Visa
        [InlineData("5500000000000004", true)]   // MasterCard
        [InlineData("340000000000009", true)]    // Amex
        [InlineData("1234567890123456", false)]  // Invalid
        public void ValidateNumber_ShouldWorkCorrectly(string cardNumber, bool expected)
        {
            Assert.Equal(expected, _service.ValidateNumber(cardNumber));
        }

        [Theory]
        [InlineData("12/25", true)]
        [InlineData("01/2099", true)]
        [InlineData("01/20", false)]  // Expired
        [InlineData("13/25", false)]  // Invalid month
        [InlineData("abcd", false)]
        public void ValidateIssueDate_ShouldWorkCorrectly(string date, bool expected)
        {
            Assert.Equal(expected, _service.ValidateIssueDate(date));
        }

        [Theory]
        [InlineData("123", true)]
        [InlineData("1234", true)]
        [InlineData("12a", false)]
        [InlineData("12", false)]
        public void ValidateCvc_ShouldWorkCorrectly(string cvc, bool expected)
        {
            Assert.Equal(expected, _service.ValidateCvc(cvc));
        }

        // ====================
        // Full Card Validation
        // ====================

        [Fact]
        public void ValidateCard_ShouldReturnValid_ForVisa()
        {
            var result = _service.ValidateCard("John Doe", "4111111111111111", "12/25", "123");

            Assert.True(result.IsValid);
            Assert.Equal(PaymentSystemType.Visa, result.CardType);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateCard_ShouldReturnValid_ForMasterCard()
        {
            var result = _service.ValidateCard("Jane Doe", "5500000000000004", "11/26", "456");

            Assert.True(result.IsValid);
            Assert.Equal(PaymentSystemType.MasterCard, result.CardType);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateCard_ShouldReturnValid_ForAmex()
        {
            var result = _service.ValidateCard("Alice Smith", "340000000000009", "10/27", "1234");

            Assert.True(result.IsValid);
            Assert.Equal(PaymentSystemType.AmericanExpress, result.CardType);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateCard_ShouldReturnErrors_ForInvalidInput()
        {
            var result = _service.ValidateCard("John123", "123456", "01/20", "12a");

            Assert.False(result.IsValid);
            Assert.Contains("Owner name is invalid.", result.Errors);
            Assert.Contains("Card number is invalid.", result.Errors);
            Assert.Contains("Card is expired or issue date invalid.", result.Errors);
            Assert.Contains("CVC is invalid.", result.Errors);
        }

        [Fact]
        public void ValidateCard_ShouldReturnErrors_ForMissingFields()
        {
            var result = _service.ValidateCard("", "", "", "");

            Assert.False(result.IsValid);
            Assert.Contains("Owner name is required.", result.Errors);
            Assert.Contains("Card number is required.", result.Errors);
            Assert.Contains("Issue date is required.", result.Errors);
            Assert.Contains("CVC is required.", result.Errors);
        }
    }
}
