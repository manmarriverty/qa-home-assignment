using CardValidation.Core.Enums;

namespace CardValidation.ViewModels
{
    public class CreditCardValidationResult
    {
        public string? Number { get; set; }
        public PaymentSystemType PaymentSystem { get; set; }
        public bool IsValid { get; set; }
    }
}
