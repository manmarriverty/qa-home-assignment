using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using CardValidation.Web; // make sure this matches your API project namespace
using Microsoft.AspNetCore.Mvc.Testing;

namespace CardValidation.IntegrationTests
{
    public class CardValidationApiTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CardValidationApiTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ValidateCard_ShouldReturnSuccess_ForValidCard()
        {
            var payload = new
            {
                Owner = "John Doe",
                CardNumber = "4111111111111111",
                IssueDate = "12/2030",
                Cvc = "123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/cards/validate", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("Visa", result);
        }

        [Fact]
        public async Task ValidateCard_ShouldReturnError_ForInvalidCard()
        {
            var payload = new
            {
                Owner = "John123",
                CardNumber = "1234567890123456",
                IssueDate = "01/2000",
                Cvc = "12a"
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/cards/validate", content);

            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
