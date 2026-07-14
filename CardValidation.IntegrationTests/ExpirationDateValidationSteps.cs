using CardValidation.Controllers;
using CardValidation.Core.Enums;
using CardValidation.Core.Services;
using CardValidation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Reqnroll;
using Xunit;

namespace CardValidation.IntegrationTests;

[Binding]
public sealed class ExpirationDateValidationSteps
{
    private CardValidationService _service = null!;
    private CardValidationController _controller = null!;
    private bool _validationResult;
    private string? _errorMessage;
    private IActionResult? _actionResult;

    [Given("I have a valid card validation service")]
    public void GivenIHaveAValidCardValidationService()
    {
        _service = new CardValidationService();
        _controller = new CardValidationController(_service);
    }

    [When("I validate the expiration date {string}")]
    public void WhenIValidateTheExpirationDate(string expirationDate)
    {
        _validationResult = _service.ValidateIssueDate(expirationDate);
        _errorMessage = _validationResult ? null : "Invalid expiration date";
    }

    [When("I validate the expiration date for next month")]
    public void WhenIValidateTheExpirationDateForNextMonth()
    {
        var nextMonth = DateTime.UtcNow.AddMonths(1);
        var expirationDate = $"{nextMonth.Month:D2}/{nextMonth.Year}";
        _validationResult = _service.ValidateIssueDate(expirationDate);
        _errorMessage = _validationResult ? null : "Invalid expiration date";
    }

    [When("I submit a credit card with expiration date {string} to the validation endpoint")]
    public async Task WhenISubmitACreditCardWithExpirationDateToTheValidationEndpoint(string expirationDate)
    {
        var card = new CreditCard
        {
            Owner = "John Doe",
            Number = "4532015112830366",
            Date = expirationDate,
            Cvv = "123"
        };

        await Task.CompletedTask;
        _actionResult = _controller.ValidateCreditCard(card);
    }

    [Then("the validation should succeed")]
    public void ThenTheValidationShouldSucceed()
    {
        Assert.True(_validationResult);
    }

    [Then("the card should be considered valid")]
    public void ThenTheCardShouldBeConsideredValid()
    {
        Assert.True(_validationResult);
    }

    [Then("the validation should fail")]
    public void ThenTheValidationShouldFail()
    {
        Assert.False(_validationResult);
    }

    [Then("the error message should indicate an invalid date")]
    public void ThenTheErrorMessageShouldIndicateAnInvalidDate()
    {
        Assert.False(string.IsNullOrWhiteSpace(_errorMessage));
        Assert.Contains("invalid", _errorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Then("I should receive a successful response")]
    public async Task ThenIShouldReceiveASuccessfulResponse()
    {
        await Task.CompletedTask;
        Assert.NotNull(_actionResult);
        Assert.IsType<OkObjectResult>(_actionResult);
    }

    [Then("the response should indicate card validation passed")]
    public async Task ThenTheResponseShouldIndicateCardValidationPassed()
    {
        await Task.CompletedTask;
        var okResult = Assert.IsType<OkObjectResult>(_actionResult);
        Assert.Equal(PaymentSystemType.Visa, okResult.Value);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _actionResult = null;
    }
}
