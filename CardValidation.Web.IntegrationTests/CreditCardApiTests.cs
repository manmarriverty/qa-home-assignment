using System;
using System.Net;
using System.Net.Http.Json;
using CardValidation.Core.Enums;
using CardValidation.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CardValidation.Web.IntegrationTests
{
    public class CreditCardApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CreditCardApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ValidVisa_Returns200_AndVisaEnumValue()
        {
            var requestBody = new
            {
                Owner = "John Doe",
                Number = "4111111111111111",
                Date = "12/30",
                Cvv = "123"
            };

            var response = await _client.PostAsJsonAsync(
                "/CardValidation/card/credit/validate",
                requestBody);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var intValue = int.Parse(content);
            var paymentType = (PaymentSystemType)intValue;

            Assert.Equal(PaymentSystemType.Visa, paymentType);
        }

        [Fact]
        public async Task InvalidData_ReturnsBadRequest_AndValidationMessages()
        {
            var requestBody = new
            {
                Owner = "",
                Number = "1234567890123456",
                Date = "01/20",
                Cvv = ""
            };

            var response = await _client.PostAsJsonAsync(
                "/CardValidation/card/credit/validate",
                requestBody);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Cvv is required", content);
        }
    }
}
