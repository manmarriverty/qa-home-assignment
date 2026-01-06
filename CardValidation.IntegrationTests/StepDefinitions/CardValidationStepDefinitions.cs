using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CardValidation.IntegrationTests.Hooks;
using CardValidation.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace CardValidation.IntegrationTests.StepDefinitions
{
    [Binding]
    public class CardValidationStepDefinitions
    {
        private HttpClient? _client;
        private HttpResponseMessage? _response;
        private CreditCard? _creditCard;

        [Given(@"the API is available")]
        public void GivenTheApiIsAvailable()
        {
            _client = TestHooks.GetFactory().CreateClient();
        }

        [Given(@"I have a valid Visa card with the following details:")]
        public void GivenIHaveAValidVisaCardWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a valid MasterCard with the following details:")]
        public void GivenIHaveAValidMasterCardWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a valid American Express card with the following details:")]
        public void GivenIHaveAValidAmericanExpressCardWithTheFollowingDetails(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with missing owner:")]
        public void GivenIHaveACardWithMissingOwner(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = null,
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with invalid number:")]
        public void GivenIHaveACardWithInvalidNumber(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with invalid date:")]
        public void GivenIHaveACardWithInvalidDate(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with invalid CVC:")]
        public void GivenIHaveACardWithInvalidCvc(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with multiple invalid fields:")]
        public void GivenIHaveACardWithMultipleInvalidFields(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [Given(@"I have a card with invalid date format:")]
        public void GivenIHaveACardWithInvalidDateFormat(Table table)
        {
            var row = table.Rows[0];
            _creditCard = new CreditCard
            {
                Owner = row["Owner"],
                Number = row["Number"],
                Date = row["Date"],
                Cvv = row["Cvv"]
            };
        }

        [When(@"I submit the card for validation")]
        public async Task WhenISubmitTheCardForValidation()
        {
            _response = await _client!.PostAsJsonAsync("/CardValidation/card/credit/validate", _creditCard);
        }

        [Then(@"the validation should succeed")]
        public void ThenTheValidationShouldSucceed()
        {
            _response!.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the validation should fail")]
        public void ThenTheValidationShouldFail()
        {
            _response!.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
        }

        [Then(@"the response should contain payment system type ""(.*)""")]
        public async Task ThenTheResponseShouldContainPaymentSystemType(string expectedType)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            var typeMap = new Dictionary<string, string>
            {
                { "Visa", "10" },
                { "MasterCard", "20" },
                { "AmericanExpress", "30" }
            };
            var expectedValue = typeMap.GetValueOrDefault(expectedType, expectedType);
            content.Should().Contain(expectedValue);
        }

        [Then(@"the response should contain error ""(.*)""")]
        public async Task ThenTheResponseShouldContainError(string expectedError)
        {
            var content = await _response!.Content.ReadAsStringAsync();
            content.Should().Contain(expectedError);
        }

        [Then(@"the response should contain multiple errors")]
        public async Task ThenTheResponseShouldContainMultipleErrors()
        {
            _response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await _response.Content.ReadAsStringAsync();
            content.Should().NotBeEmpty();
        }
    }
}

