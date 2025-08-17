using CardValidation.Core.Services.Interfaces;
using CardValidation.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CardValidation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardValidationController : ControllerBase
    {
        private readonly ICardValidationService cardValidator;

        public CardValidationController(ICardValidationService cardValidationService)
        {
            cardValidator = cardValidationService;
        }

        [HttpPost("card/credit/validate")]
        public IActionResult ValidateCreditCard([FromBody] CreditCard creditCard)
        {
            if (creditCard == null)
                return BadRequest(new { Error = "Invalid credit card input." });

            var result = cardValidator.ValidateCard(
                creditCard.Owner,
                creditCard.Number ?? string.Empty,
                creditCard.IssueDate,
                creditCard.Cvc
            );

            if (!result.IsValid)
                return BadRequest(new { Errors = result.Errors });

            return Ok(new { CardType = result.CardType?.ToString() });
        }
    }
}
