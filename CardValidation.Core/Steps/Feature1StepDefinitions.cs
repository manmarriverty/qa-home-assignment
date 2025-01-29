using Reqnroll;
using Xunit;
using Newtonsoft.Json;
using System.Text;

namespace CardValidation.Core
{
    [Binding]
    public class Feature1StepDefinitions
    {
        private readonly HttpClient client;
        private object requestBody;
        private dynamic response;


        public Feature1StepDefinitions()
        {

            client = new HttpClient();
        }

       
        [Given("the user provides with following details")]
        public void GivenTheUserProvidesWithFollowingDetails()
        {
            requestBody = new
            {

                Owner = "John Doe",
                Number = "4111111111111111",
                Date = "12/2025",
                Cvv = "123"

            };
        }

    
        [When("the user submit card details to API")]
        public async Task WhenTheUserSubmitCardDetailsToAPI()
        {

            using (var client = new HttpClient())
            {
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                response = await client.PostAsync("https://localhost:7135/cardvalidation/card/credit/validate", content);
            }
        }
        
        [Then("the response is Visa")]
        public async Task ThenTheResponseIsVisa()
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode, "Request failed with status code: " + response.StatusCode);

            Assert.True(responseContent.Contains("10"), "Response content doesn't contain the expected string.");

        }
    }
}
