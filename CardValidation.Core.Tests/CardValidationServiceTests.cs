using System;
using CardValidation.Core.Enums;
using CardValidation.Core.Services;
using Xunit;

namespace CardValidation.Core.Tests
{
    public class CardValidationServiceTests
    {
        private readonly CardValidationService _sut = new CardValidationService();

        // ValidateOwner

        [Theory]
        [InlineData("John")]
        [InlineData("John Doe")]
        [InlineData("Jane Mary Smith")]   // 3 words is allowed
        public void ValidateOwner_ValidNames_ReturnsTrue(string owner)
        {
            // act
            var result = _sut.ValidateOwner(owner);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("12345")]
        [InlineData("John Doe !!!")]      // invalid chars
        [InlineData("Very Very Long Name Here")] // > 3 words
        public void ValidateOwner_InvalidNames_ReturnsFalse(string owner)
        {
            var result = _sut.ValidateOwner(owner);

            Assert.False(result);
        }

        // ValidateIssueDate

        [Theory]
        // 2-digit year, clearly future
        [InlineData("12/30")]     // Dec 2030
        // 4-digit year, clearly future
        [InlineData("01/2035")]   // Jan 2035
        public void ValidateIssueDate_FutureDates_ReturnTrue(string issueDate)
        {
            var result = _sut.ValidateIssueDate(issueDate);

            Assert.True(result);
        }

        [Theory]
        [InlineData("01/20")]     // Jan 2020 – past
        [InlineData("11/2010")]   // Nov 2010 – past
        [InlineData("13/30")]     // invalid month
        [InlineData("00/25")]     // invalid month
        [InlineData("1/25")]      // wrong format
        [InlineData("abcd")]      // junk
        public void ValidateIssueDate_PastOrInvalidDates_ReturnFalse(string issueDate)
        {
            var result = _sut.ValidateIssueDate(issueDate);

            Assert.False(result);
        }

        // ValidateCvc

        [Theory]
        [InlineData("123")]
        [InlineData("999")]
        [InlineData("000")]
        [InlineData("1234")]      // Amex-style 4-digit CVC
        public void ValidateCvc_ValidValues_ReturnTrue(string cvc)
        {
            var result = _sut.ValidateCvc(cvc);

            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("12345")]
        [InlineData("12a")]
        [InlineData("abc")]
        public void ValidateCvc_InvalidValues_ReturnFalse(string cvc)
        {
            var result = _sut.ValidateCvc(cvc);

            Assert.False(result);
        }

        // ValidateNumber

        [Theory]
        // Visa test number
        [InlineData("4111111111111111")]
        // MasterCard test number
        [InlineData("5555555555554444")]
        // American Express test number
        [InlineData("378282246310005")]
        public void ValidateNumber_ValidKnownNumbers_ReturnTrue(string cardNumber)
        {
            var result = _sut.ValidateNumber(cardNumber);

            Assert.True(result);
        }

        [Theory]
        [InlineData("1234567890123456")]
        [InlineData("0000000000000000")]
        [InlineData("")]
        [InlineData("411111111111111")]   // too short
        [InlineData("41111111111111111")] // too long
        public void ValidateNumber_InvalidNumbers_ReturnFalse(string cardNumber)
        {
            var result = _sut.ValidateNumber(cardNumber);

            Assert.False(result);
        }

 
        // GetPaymentSystemType

        [Fact]
        public void GetPaymentSystemType_VisaNumber_ReturnsVisa()
        {
            var type = _sut.GetPaymentSystemType("4111111111111111");

            Assert.Equal(PaymentSystemType.Visa, type);
        }

        [Fact]
        public void GetPaymentSystemType_MasterCardNumber_ReturnsMasterCard()
        {
            var type = _sut.GetPaymentSystemType("5555555555554444");

            Assert.Equal(PaymentSystemType.MasterCard, type);
        }

        [Fact]
        public void GetPaymentSystemType_AmexNumber_ReturnsAmericanExpress()
        {
            var type = _sut.GetPaymentSystemType("378282246310005");

            Assert.Equal(PaymentSystemType.AmericanExpress, type);
        }

        [Fact]
        public void GetPaymentSystemType_InvalidNumber_ThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() =>
                _sut.GetPaymentSystemType("1234567890123456"));
        }
    }
}
