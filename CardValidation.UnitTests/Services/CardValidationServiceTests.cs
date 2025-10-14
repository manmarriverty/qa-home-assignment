using CardValidation.Core.Enums;
using CardValidation.Core.Services;

namespace CardValidation.UnitTests.Services
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService cardValidationService;

        public CardValidationServiceTests()
        {
            cardValidationService = new CardValidationService();
        }

        [Theory]
        [InlineData("4111111111111111")] // Visa
        [InlineData("5555555555554444")] // MasterCard
        [InlineData("378282246310005")]  // AmericanExpress
        public void verifyCardValidationReturnsTrueForValidCardNumber(string cardNumber)
        {
            Assert.True(cardValidationService.ValidateNumber(cardNumber));
        }

        [Fact]
        public void verifyCardValidationReturnsFalseForInvalidCard()
        {
            Assert.False(cardValidationService.ValidateNumber("60111111111"));
        }

        [Fact]
        public void verifyCvcValidationReturnsTrueForValidCvc()
        {
            Assert.True(cardValidationService.ValidateCvc("459"));
        }

        [Fact]
        public void verifyCvcValidationReturnsFalseForInvalidCvc()
        {
            Assert.False(cardValidationService.ValidateCvc("2"));
        }

        [Fact]
        public void verifyOwnerValidationReturnsTrueForValidName()
        {
            Assert.True(cardValidationService.ValidateOwner("John Doe"));
        }

        [Fact]
        public void verifyOwnerValidationReturnsFalseForInvalidname()
        {
            Assert.False(cardValidationService.ValidateOwner("2556@@@@gfhfg"));
        }

        [Fact]
        public void verifyIssueDateValidationReturnsTrueForValidFutureFullYear()
        {
            Assert.True(cardValidationService.ValidateIssueDate("12/2099"));
        }

        [Fact]
        public void verifyIssueDateValidationReturnsTrueForValidFutureShortYear()
        {
            Assert.True(cardValidationService.ValidateIssueDate("03/30"));
        }

        [Fact]
        public void verifyIssueDateValidationReturnsFalseForValidPastFullYear()
        {
            Assert.False(cardValidationService.ValidateIssueDate("01/2020"));
        }

        [Fact]
        public void verifyIssueDateValidationReturnsFalseForInvalidFormat()
        {
            Assert.False(cardValidationService.ValidateIssueDate("3/25"));
        }

        [Fact]
        public void verifyIssueDateValidationReturnsFalseForEmpty()
        {
            Assert.False(cardValidationService.ValidateIssueDate(""));
        }

        [Fact]
        public void verifyPaymentSystemTypeReturnsMastercardForValidMastercard()
        {
            var type = cardValidationService.GetPaymentSystemType("4111111111111111");
            Assert.Equal(PaymentSystemType.MasterCard, type);
        }

        [Fact]
        public void verifyPaymentSystemTypeReturnsVisaForValidVisa()
        {
            var type = cardValidationService.GetPaymentSystemType("5555555555554444");
            Assert.Equal(PaymentSystemType.Visa, type);
        }

        [Fact]
        public void verifyPaymentSystemTypeReturnsAmericanExpressForValidAmericanExpress()
        {
            var type = cardValidationService.GetPaymentSystemType("378282246310005");
            Assert.Equal(PaymentSystemType.AmericanExpress, type);
        }

        [Fact]
        public void verifyPaymentSystemTypeThrowsNotImplementedExceptionForOtherCardType()
        {
            Assert.Throws<NotImplementedException>(() => cardValidationService.GetPaymentSystemType("6011111111111117"));
        }
    }
}
