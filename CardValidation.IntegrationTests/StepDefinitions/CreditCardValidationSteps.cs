using System.Net.Http.Json;
using CardValidation.Core.Enums;
using CardValidation.IntegrationTests.Models;
using CardValidation.ViewModels;
using FluentAssertions;
using Reqnroll;

namespace CardValidation.IntegrationTests.StepDefinitions
{
    [Binding]
    public class CreditCardValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HttpClient _httpClient;
        private CreditCardTestModel? _testCard;
        private HttpResponseMessage? _response;

        public CreditCardValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _httpClient = (HttpClient)_scenarioContext["HttpClient"];
        }

        [Given(@"I have a credit card with the following data:")]
        public void GivenIHaveACreditCardWithTheFollowingData(Table table)
        {
            _testCard = table.CreateInstance<CreditCardTestModel>();
        }

        [When(@"I send the validation request")]
        public async Task WhenISendTheValidationRequest()
        {
            var card = new CreditCard
            {
                Owner = _testCard?.Owner,
                Number = _testCard?.Number,
                Date = _testCard?.Date,
                Cvv = _testCard?.Cvv
            };

            _response = await _httpClient.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            _scenarioContext.Add("Response", _response);
        }

        [Then(@"I should receive status code (.*)")]
        public void ThenIShouldReceiveStatusCode(int statusCode)
        {
            _response?.StatusCode.Should().Be((System.Net.HttpStatusCode)statusCode);
        }

        [Then(@"the card type should be ""(.*)""")]
        public async Task ThenTheCardTypeShouldBe(string cardType)
        {
            var content = await _response!.Content.ReadFromJsonAsync<PaymentSystemType>();
            content.ToString().Should().Be(cardType);
        }
    }
}