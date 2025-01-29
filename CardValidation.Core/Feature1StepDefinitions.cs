using System;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;  // For Debug.WriteLine
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

        [Fact]
        [Given("the user provides with following details")]
        public async Task GivenTheUserProvidesWithFollowingDetails()
        {
            requestBody = new
            {

                Owner = "John Doe",
                Number = "4111111111111111", // Valid card number for testing
                Date = "12/2025",  // Correct expiry date format
                Cvv = "123"

            };

            using (var client = new HttpClient())
            {
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7135/cardvalidation/card/credit/validate", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.True(response.IsSuccessStatusCode, "hello");
            }
            

        }

        [Fact]
        [When("the user submit card details to API")]
        public async Task WhenTheUserSubmitCardDetailsToAPI()
        {
          

        }

        [Fact]
        [Then("the response is Visa")]
        public async Task ThenTheResponseIsVisa()
        {
           

        }
    }
}
