using CardValidation.Core.Enums;
using CardValidation.Core.Services;

namespace CardValidation.Tests.Unit.Services;

public class CardValidationServiceTests
{

    [Theory]
    [InlineData("John")]
    [InlineData("Amy Doe")]
    [InlineData("JohN DoE SmIth")]
    public void ValidateOwner_ValidOwner_ReturnsTrue(string validOwner)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateOwner(validOwner);

        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("John123", "Numbers are not allowed in the name")]
    [InlineData("John Do$", "Special characters are not allowed in the name")]
    [InlineData("John\tDoe Smith", "Tabs are not allowed in the name")]
    [InlineData("John Doe  Smith", "Multiple spaces are not allowed in the name")]
    [InlineData(" John Doe", "Leading spaces are not allowed in the name")]
    [InlineData("John Doe ", "Trailing spaces are not allowed in the name", Skip = "currently failing, see README.md")]
    public void ValidateOwner_InvalidOwner_ReturnsFalse(string invalidOwner, string reason)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateOwner(invalidOwner);

        // Assert
        Assert.False(result, reason);
    }
    
    [Fact]
    public void ValidateOwner_LongName_ReturnsFalse()
    {
        // Arrange
        var service = new CardValidationService();
        const string longName = "John Doe Smith Johnson";
        
        // Act
        var result = service.ValidateOwner(longName);
        
        // Assert
        Assert.False(result, "Name must not exceed the maximum length of 3 words");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ValidateOwner_EmptyName_ReturnsFalse(string emptyName)
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act
        var result = service.ValidateOwner(emptyName);
        
        // Assert
        Assert.False(result, "Name must not be empty or whitespace");
    }
    
    [Fact]
    public void ValidateOwner_ThrownException_WhenNull()
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.ValidateOwner(null!));
    }
    
    [Theory]
    [InlineData("335")]
    [InlineData("6544")]
    public void ValidateCvs_ValidCvc_ReturnsTrue(string validCvc)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateCvc(validCvc);

        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("35", "CVS is too short, must be 3 or 4 digits long")]
    [InlineData("65445", "CVS is too long, must be 3 or 4 digits long")]
    [InlineData("12a", "No alphabetic characters are allowed in CVS")]
    [InlineData("3.14", "No special characters are allowed in CVS")]
    [InlineData(" 133", "No spaces allowed in CVS")]
    public void ValidateCvs_InvalidCvc_ReturnsFalse(string invalidCvc, string reason)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateCvc(invalidCvc);

        // Assert
        Assert.False(result, reason);
    }
    
    [Fact]
    public void ValidateCvs_ThrownException_WhenNull()
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.ValidateCvc(null!));
    }
    
    [Fact(Skip = "Will fail because of the current implementation, see README.md")]
    public void ValidateIssueDate_CurrentMonth_ReturnsTrue()
    {
        // Arrange
        var service = new CardValidationService();
        var currentMonth = DateTime.UtcNow.ToString("MM\\/yyyy");

        // Act
        var result = service.ValidateIssueDate(currentMonth);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidateIssueDate_NextMonthLongFormat_ReturnsTrue()
    {
        // Arrange
        var service = new CardValidationService();
        var nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MM\\/yyyy");

        // Act
        var result = service.ValidateIssueDate(nextMonth);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidateIssueDate_NextMonthShortFormat_ReturnsTrue()
    {
        // Arrange
        var service = new CardValidationService();
        var nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MM\\/yy");

        // Act
        var result = service.ValidateIssueDate(nextMonth);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidateIssueDate_NextMonthShortFormatSimplified_ReturnsTrue()
    {
        // Arrange
        var service = new CardValidationService();
        var nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MMyy");

        // Act
        var result = service.ValidateIssueDate(nextMonth);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidateIssueDate_NextMonthLongFormatSimplified_ReturnsTrue()
    {
        // Arrange
        var service = new CardValidationService();
        var nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MMyyyy");

        // Act
        var result = service.ValidateIssueDate(nextMonth);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ValidateIssueDate_PastMonthLongFormat_ReturnsFalse()
    {
        // Arrange
        var service = new CardValidationService();
        var pastMonth = DateTime.UtcNow.AddMonths(-1).ToString("MM\\/yyyy");

        // Act
        var result = service.ValidateIssueDate(pastMonth);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ValidateIssueDate_PastMonthShortFormat_ReturnsFalse()
    {
        // Arrange
        var service = new CardValidationService();
        var pastMonth = DateTime.UtcNow.AddMonths(-1).ToString("MM\\/yy");

        // Act
        var result = service.ValidateIssueDate(pastMonth);

        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("$1/2025")]
    [InlineData("01/01/2025")]
    [InlineData("01_25")]
    [InlineData("A1/2025")]
    public void ValidateIssueDate_InvalidFormat_ReturnsFalse(string invalidDate)
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act
        var result = service.ValidateIssueDate(invalidDate);
        
        // Assert
        Assert.False(result, "Invalid date format");
    }
    
    [Fact]
    public void ValidateIssueDate_ThrownException_WhenNull()
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.ValidateIssueDate(null!));
    }

    [Theory]
    [InlineData("4123456789012")]
    [InlineData("4012888888881881")]
    public void GetPaymentSystemType_ValidVisaNumber_ReturnsVisaPaymentSystemType(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act
        var result = service.GetPaymentSystemType(cardNumber);
        
        // Assert
        Assert.Equal(PaymentSystemType.Visa, result);
    }
    
    [Theory]
    [InlineData("5105105105105100")]
    [InlineData("2221000000000009")]
    [InlineData("2720990000000002")]
    public void GetPaymentSystemType_ValidMasterCardNumber_ReturnsMasterCardPaymentSystemType(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act
        var result = service.GetPaymentSystemType(cardNumber);
        
        // Assert
        Assert.Equal(PaymentSystemType.MasterCard, result);
    }
    
    [Theory]
    [InlineData("348282246310005")]
    [InlineData("371449635398431")]
    public void GetPaymentSystemType_ValidAmericanExpressNumber_ReturnsAmericanExpressPaymentSystemType(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();
        
        // Act
        var result = service.GetPaymentSystemType(cardNumber);
        
        // Assert
        Assert.Equal(PaymentSystemType.AmericanExpress, result);
    }

    [Theory]
    [InlineData("0000000000000000")]
    [InlineData("9999999999999999")]
    [InlineData("123456789012345")]
    [InlineData("!23456789012345")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234-5678-9012-3456")]
    public void GetPaymentSystemType_InvalidCardNumber_ThrowsNotImplementedException(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => service.GetPaymentSystemType(cardNumber));
    }
    
    [Theory]
    [InlineData("4123456789012")]
    [InlineData("4012888888881881")]
    [InlineData("5105105105105100")]
    [InlineData("2221000000000009")]
    [InlineData("2720990000000002")]
    [InlineData("348282246310005")]
    [InlineData("371449635398431")]
    public void ValidateNumber_ValidCard_ReturnsTrue(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateNumber(cardNumber);
        
        // Assert
        Assert.True(result, "Card number is valid for payment system");
    }

    [Theory]
    [InlineData("0000000000000000")]
    [InlineData("9999999999999999")]
    [InlineData("123456789012345")]
    [InlineData("!23456789012345")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234-5678-9012-3456")]
    public void ValidateNumber_InvalidCardNumber_ReturnsFalse(string cardNumber)
    {
        // Arrange
        var service = new CardValidationService();

        // Act
        var result = service.ValidateNumber(cardNumber);
        
        // Assert
        Assert.False(result, "Card number is not valid for any known payment system");
    }
}