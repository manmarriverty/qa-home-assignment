using CardValidation.Core.Enums;
using CardValidation.Core.Services;
using FluentAssertions;

namespace CardValidation.Core.Tests.Services
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService _sut;

        public CardValidationServiceTests()
        {
            _sut = new CardValidationService();
        }

        public class ValidateOwnerTests : CardValidationServiceTests
        {
            [Theory]
            [InlineData("João Silva")]
            [InlineData("Maria Santos")]
            [InlineData("José")]
            [InlineData("Ana Maria Silva")]
            public void WhenOwnerNameIsValid_ShouldReturnTrue(string ownerName)
            {
                var result = _sut.ValidateOwner(ownerName);

                result.Should().BeTrue();
            }

            [Theory]
            [InlineData("")]
            [InlineData("João123")]
            [InlineData("Maria Silva Santos Costa")] // More then 3 words
            [InlineData("J0ão")]
            public void WhenOwnerNameIsInvalid_ShouldReturnFalse(string ownerName)
            {
                var result = _sut.ValidateOwner(ownerName);

                result.Should().BeFalse();
            }
        }

        public class ValidateIssueDateTests : CardValidationServiceTests
        {
            [Theory]
            [InlineData("12/25")]
            [InlineData("01/2030")]
            public void WhenDateIsValid_AndInFuture_ShouldReturnTrue(string issueDate)
            {
                var result = _sut.ValidateIssueDate(issueDate);

                result.Should().BeTrue();
            }

            [Theory]
            [InlineData("13/25")] // Invalid month case 1
            [InlineData("00/25")] // Invalid month case 2
            [InlineData("12-25")] // Invalid format
            [InlineData("12/20")] // Past data
            public void WhenDateIsInvalid_ShouldReturnFalse(string issueDate)
            {
                var result = _sut.ValidateIssueDate(issueDate);

                result.Should().BeFalse();
            }
        }

        public class ValidateCvcTests : CardValidationServiceTests
        {
            [Theory]
            [InlineData("123")]
            [InlineData("1234")]
            public void WhenCvcIsValid_ShouldReturnTrue(string cvc)
            {
                var result = _sut.ValidateCvc(cvc);

                result.Should().BeTrue();
            }

            [Theory]
            [InlineData("12")]
            [InlineData("12345")]
            [InlineData("abc")]
            [InlineData("")]
            public void WhenCvcIsInvalid_ShouldReturnFalse(string cvc)
            {
                var result = _sut.ValidateCvc(cvc);

                result.Should().BeFalse();
            }
        }

        public class ValidateNumberTests : CardValidationServiceTests
        {
            [Theory]
            [InlineData("4532123456788901")] // Visa
            [InlineData("5412345678901234")] // MasterCard
            [InlineData("341234567890123")]  // American Express
            public void WhenCardNumberIsValid_ShouldReturnTrue(string cardNumber)
            {
                var result = _sut.ValidateNumber(cardNumber);

                result.Should().BeTrue();
            }

            [Theory]
            [InlineData("1234567890123456")]
            [InlineData("")]
            [InlineData("abc")]
            public void WhenCardNumberIsInvalid_ShouldReturnFalse(string cardNumber)
            {
                var result = _sut.ValidateNumber(cardNumber);

                result.Should().BeFalse();
            }
        }

        public class GetPaymentSystemTypeTests : CardValidationServiceTests
        {
            [Theory]
            [InlineData("4532123456788901", PaymentSystemType.Visa)]
            [InlineData("5412345678901234", PaymentSystemType.MasterCard)]
            [InlineData("341234567890123", PaymentSystemType.AmericanExpress)]
            public void WhenCardNumberIsValid_ShouldReturnCorrectType(string cardNumber, PaymentSystemType expectedType)
            {
                var result = _sut.GetPaymentSystemType(cardNumber);

                result.Should().Be(expectedType);
            }

            [Fact]
            public void WhenCardNumberIsInvalid_ShouldThrowNotImplementedException()
            {
                var invalidCard = "1234567890123456";

                Assert.Throws<NotImplementedException>(() => _sut.GetPaymentSystemType(invalidCard));
            }
        }
    }
}