using CardValidation.Core.Services.Interfaces;
using CardValidation.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CardValidation.Infrustructure
{
    public class CreditCardValidationFilter : IActionFilter
    {
        private readonly ICardValidationService cardValidationService;

        private static void AddParameterIsRequiredError(ActionExecutingContext context, string parameterName)
            => context.ModelState.AddModelError(parameterName, $"{parameterName} is required");

        private static void AddWrongParameterError(ActionExecutingContext context, string parameterName)
            => context.ModelState.AddModelError(parameterName, $"Wrong {parameterName.ToLowerInvariant()}");

        public CreditCardValidationFilter(ICardValidationService cardValidationService)
        {
            this.cardValidationService = cardValidationService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments != null && context.ActionArguments.Count > 0)
            {
                if (context.ActionArguments.TryGetValue("creditCard", out object? value))
                {
                    var card = value as CreditCard;

                    if (card != null)
                    {
                        ValidateParameter(context, nameof(card.Owner), card.Owner, cardValidationService.ValidateOwner);
                        ValidateParameter(context, nameof(card.IssueDate), card.IssueDate, cardValidationService.ValidateIssueDate);
                        ValidateParameter(context, nameof(card.Cvc), card.Cvc, cardValidationService.ValidateCvc);
                        ValidateParameter(context, nameof(card.Number), card.Number, cardValidationService.ValidateNumber);
                    }
                    else
                    {
                        throw new InvalidOperationException("CreditCard parameter is invalid.");
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No-op
        }

        private static void ValidateParameter(ActionExecutingContext context, string name, string? value, Func<string, bool> isParameterValid)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddParameterIsRequiredError(context, name);
            }
            else if (!isParameterValid(value))
            {
                AddWrongParameterError(context, name);
            }
        }
    }
}
