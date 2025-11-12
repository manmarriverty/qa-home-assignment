using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CardValidation.Core.Enums;
using CardValidation.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;
using Xunit;

namespace CardValidation.Web.ReqnrollTests
{
    [Binding]
    public class CreditCardSteps
    {
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private object _requestBody;
        private HttpResponseMessage _response;

        public CreditCardSteps()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        // ---------- Givens ----------

        [Given("I have a valid Visa credit card")]
        public void GivenIHaveAValidVisaCreditCard()
        {
            _requestBody = new
            {
                Owner = "John Doe",
                Number = "4111111111111111", // Visa
                Date = "12/30",
                Cvv = "123"
            };
        }

        [Given("I have a valid MasterCard credit card")]
        public void GivenIHaveAValidMasterCardCreditCard()
        {
            _requestBody = new
            {
                Owner = "John Doe",
                Number = "5555555555554444", // MasterCard
                Date = "12/30",
                Cvv = "123"
            };
        }

        [Given("I have an invalid credit card with missing Cvv")]
        public void GivenIHaveAnInvalidCreditCardWithMissingCvv()
        {
            _requestBody = new
            {
                Owner = "",
                Number = "1234567890123456",
                Date = "01/20",
                Cvv = ""
            };
        }

        // ---------- When ----------

        [When("I send the validation request")]
        public async Task WhenISendTheValidationRequest()
        {
            _response = await _client.PostAsJsonAsync(
                "/CardValidation/card/credit/validate",
                _requestBody);
        }

        // ---------- Thens: status codes ----------

        [Then("the response status code should be 200")]
        public void ThenTheResponseStatusCodeShouldBe200()
        {
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
        }

        [Then("the response status code should be 400")]
        public void ThenTheResponseStatusCodeShouldBe400()
        {
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        }

        // ---------- Thens: payment system type ----------

        [Then("the payment system type should be Visa")]
        public async Task ThenThePaymentSystemTypeShouldBeVisa()
        {
            var content = await _response.Content.ReadAsStringAsync();
            var intValue = int.Parse(content);
            var paymentType = (PaymentSystemType)intValue;

            Assert.Equal(PaymentSystemType.Visa, paymentType);
        }

        [Then("the payment system type should be MasterCard")]
        public async Task ThenThePaymentSystemTypeShouldBeMasterCard()
        {
            var content = await _response.Content.ReadAsStringAsync();
            var intValue = int.Parse(content);
            var paymentType = (PaymentSystemType)intValue;

            Assert.Equal(PaymentSystemType.MasterCard, paymentType);
        }

        // ---------- Thens: validation message ----------

        [Then("the response should contain a Cvv validation message")]
        public async Task ThenTheResponseShouldContainACvvValidationMessage()
        {
            var content = await _response.Content.ReadAsStringAsync();
            Assert.Contains("Cvv is required", content);
        }
    }
}
