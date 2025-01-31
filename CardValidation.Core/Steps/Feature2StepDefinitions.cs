using System;
using Newtonsoft.Json;
using System.Text;
using Reqnroll;
using Xunit;
using Newtonsoft.Json.Linq;

namespace CardValidation.Core.Steps
{
    [Binding]
    public class Feature2StepDefinitions
    {
        private readonly HttpClient client;
        private object requestBody;
        private dynamic response;

        public Feature2StepDefinitions()
        {
            client = new HttpClient();
        }

        [Given("the user provide card with missing field")]
        public void GivenTheUserProvideCardWithMissingField()
        {
            requestBody = new
            {
                Owner = "",
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "123"
            };
        }

        [When("the user submit the card detail to API")]
        public async Task WhenTheUserSubmitTheCardDetailToAPI()
        {
            using (var client = new HttpClient())
            {
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                response = await client.PostAsync("https://localhost:7135/cardvalidation/card/credit/validate", content);
            }
        }

        [Then("the response shows Owner is required")]
        public async Task ThenTheResponseShowsOwnerIsRequired()
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(400, (int)response.StatusCode);
            var jsonResponse = JObject.Parse(responseContent);

            var ownerErrors = jsonResponse["Owner"];
            Assert.NotNull(ownerErrors);
            Assert.IsType<JArray>(ownerErrors);
            Assert.Contains("Owner is required", ownerErrors.ToObject<string[]>());
        }
    }
}
