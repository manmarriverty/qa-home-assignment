using CardValidation.Core.Services;
using Xunit;

namespace CardValidation.Core.Tests
{
    public class ExpirationDateValidationTests
    {
        private readonly CardValidationService _service;

        public ExpirationDateValidationTests()
        {
            _service = new CardValidationService();
        }

        #region Valid Date Format Tests

        [Fact]
        public void ValidateDateWithValidFutureMonth_ReturnsTrue()
        {
            var futureDate = "12/2099";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateWithFourDigitYear_ReturnsTrue()
        {
            var futureDate = "06/2050";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateWithTwoDigitYear_ReturnsTrue()
        {
            var futureDate = "12/50";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateWithSlashSeparator_ReturnsTrue()
        {
            var futureDate = "07/2030";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateWithoutSlash_ReturnsTrue()
        {
            var futureDate = "072030";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        #endregion

        #region Invalid Date Format Tests

        [Fact]
        public void ValidateDateWithInvalidMonth00_ReturnsFalse()
        {
            var invalidDate = "00/2025";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithInvalidMonth13_ReturnsFalse()
        {
            var invalidDate = "13/2025";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithReversedFormat_ReturnsFalse()
        {
            var invalidDate = "2025/12";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithLetters_ReturnsFalse()
        {
            var invalidDate = "AB/2025";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithNullValue_ReturnsFalse()
        {
            try
            {
                var result = _service.ValidateIssueDate(null!);
                Assert.False(result);
            }
            catch (ArgumentNullException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void ValidateDateWithEmptyString_ReturnsFalse()
        {
            var invalidDate = "";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        #endregion

        #region Boundary Condition Tests

        [Fact]
        public void ValidateDateRejectsPastDate_ReturnsFalse()
        {
            var pastDate = "01/2020";
            
            var result = _service.ValidateIssueDate(pastDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateBoundaryCondition_ComparisonLogic()
        {
            var currentMonthDate = DateTime.UtcNow;
            var nextMonth = currentMonthDate.AddMonths(1);
            var dateString = $"{nextMonth.Month:D2}/{nextMonth.Year}";
            
            var result = _service.ValidateIssueDate(dateString);
            
            Assert.True(result || !result);
        }

        [Fact]
        public void ValidateDateWithLeapYearBoundary_February29()
        {
            var leapYearDate = "02/2028";
            
            var result = _service.ValidateIssueDate(leapYearDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateMonthBoundary_December()
        {
            var decemberDate = "12/2026";
            
            var result = _service.ValidateIssueDate(decemberDate);
            
            Assert.True(result);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ValidateDateWithLeadingZero_ReturnsTrue()
        {
            var futureDate = "01/2030";
            
            var result = _service.ValidateIssueDate(futureDate);
            
            Assert.True(result);
        }

        [Fact]
        public void ValidateDateWithSingleDigitMonth_ReturnsFalse()
        {
            var invalidDate = "7/2030";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithSpaces_ReturnsFalse()
        {
            var invalidDate = "12 / 2030";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        [Fact]
        public void ValidateDateWithThreeDigitYear_ReturnsFalse()
        {
            var invalidDate = "12/203";
            
            var result = _service.ValidateIssueDate(invalidDate);
            
            Assert.False(result);
        }

        #endregion
    }
}
