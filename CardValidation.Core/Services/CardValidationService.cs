using CardValidation.Core.Enums;
using CardValidation.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CardValidation.Core.Services
{
    public class CardValidationResult
    {
        public bool IsValid { get; set; }
        public PaymentSystemType? CardType { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class CardValidationService : ICardValidationService
    {
        private static bool IsVisa(string cardNumber) => Regex.IsMatch(cardNumber, @"^4[0-9]{12}(?:[0-9]{3})?$");

        private static bool IsMasterCard(string cardNumber) =>
            Regex.IsMatch(cardNumber, @"^(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}$");

        private static bool IsAmericanExpress(string cardNumber) => Regex.IsMatch(cardNumber, @"^3[47][0-9]{13}$");

        public bool ValidateOwner(string owner) =>
            !string.IsNullOrWhiteSpace(owner) && Regex.IsMatch(owner, @"^((?:[A-Za-z]+ ?){1,3})$");

        public bool ValidateIssueDate(string issueDate)
        {
            var pattern = @"^(0[1-9]|1[0-2])\/?([0-9]{2}|[0-9]{4})$";
            if (!Regex.IsMatch(issueDate, pattern)) return false;

            var match = Regex.Match(issueDate, pattern);
            int month = int.Parse(match.Groups[1].Value);
            int year = int.Parse(match.Groups[2].Value);
            if (year < 100) year += 2000;

            var expiry = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return DateTime.UtcNow <= expiry;
        }

        public bool ValidateCvc(string cvc) => Regex.IsMatch(cvc, @"^[0-9]{3,4}$");

        public bool ValidateNumber(string cardNumber) =>
            IsVisa(cardNumber) || IsMasterCard(cardNumber) || IsAmericanExpress(cardNumber);

        public PaymentSystemType GetPaymentSystemType(string cardNumber)
        {
            if (IsVisa(cardNumber)) return PaymentSystemType.Visa;
            if (IsMasterCard(cardNumber)) return PaymentSystemType.MasterCard;
            if (IsAmericanExpress(cardNumber)) return PaymentSystemType.AmericanExpress;

            return default; // PaymentSystemType.Unknown removed since enum has no Unknown
        }

        public CardValidationResult ValidateCard(string owner, string cardNumber, string issueDate, string cvc)
        {
            var result = new CardValidationResult();

            if (string.IsNullOrWhiteSpace(owner))
                result.Errors.Add("Owner name is required.");
            else if (!ValidateOwner(owner))
                result.Errors.Add("Owner name is invalid.");

            if (string.IsNullOrWhiteSpace(cardNumber))
                result.Errors.Add("Card number is required.");
            else if (!ValidateNumber(cardNumber))
                result.Errors.Add("Card number is invalid.");

            if (string.IsNullOrWhiteSpace(issueDate))
                result.Errors.Add("Issue date is required.");
            else if (!ValidateIssueDate(issueDate))
                result.Errors.Add("Card is expired or issue date invalid.");

            if (string.IsNullOrWhiteSpace(cvc))
                result.Errors.Add("CVC is required.");
            else if (!ValidateCvc(cvc))
                result.Errors.Add("CVC is invalid.");

            if (!result.Errors.Any())
            {
                result.IsValid = true;
                result.CardType = GetPaymentSystemType(cardNumber);
            }

            return result;
        }
    }
}
