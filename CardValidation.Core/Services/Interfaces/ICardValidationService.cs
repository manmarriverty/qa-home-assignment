using CardValidation.Core.Enums;

namespace CardValidation.Core.Services.Interfaces
{
    public interface ICardValidationService
    {
        // Individual validation methods
        bool ValidateOwner(string owner);
        bool ValidateNumber(string cardNumber);
        bool ValidateIssueDate(string issueDate);
        bool ValidateCvc(string cvc);

        // Get card type
        PaymentSystemType GetPaymentSystemType(string cardNumber);

        // Full card validation
        CardValidationResult ValidateCard(string owner, string cardNumber, string issueDate, string cvc);
    }
}

