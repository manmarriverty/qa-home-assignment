using System;
using System.Net.Http.Json;
using Reqnroll;
using Xunit;

namespace CardValidation.Core
{
    [Binding]
    public class Feature1StepDefinitions
    {
        private HttpClient client;
        private object requestBody;
        private HttpResponseMessage response;

        public Feature1StepDefinitions() => client = new HttpClient();

        [Fact]
        [Given("the user provides with following details")]
        public void GivenTheUserProvidesWithFollowingDetails()
        {
            var requestBody = new
            {
                Owner = "John Doe",
                Number = "4111111111110",  // Invalid card number
                Date = "01/2020",  // Expired card
                Cvv = "123"
            };
        }
        [Fact]
        [When("the user submit card details to API")]
        public void WhenTheUserSubmitCardDetailsToAPI()
        {

            var response = client.PostAsync("https://localhost:7135/swagger/index.html/CardValidation/card/credit/validate",
                JsonContent.Create(requestBody));
        }
        [Fact]
        [Then("the response is Visa")]
        public void ThenTheResponseIsVisa()
        {
            if (response == null)
            {
                throw new Exception("Response is null.");
            }
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Expected status code 200, but got {response.StatusCode}");
            }
        }
    }
}
