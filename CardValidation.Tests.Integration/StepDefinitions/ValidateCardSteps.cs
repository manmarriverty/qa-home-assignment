using System.Text;
using System.Text.Json;
using CardValidation.Core.Enums;
using CardValidation.ViewModels;

namespace CardValidation.Tests.Integration.StepDefinitions
{
    [Binding]
    public class ValidateCardSteps
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;
        private string? _responseBody;
        private string? _currentEndpoint;
        private JsonElement? _responseJson;
        private string? _dateFormat;

        public ValidateCardSteps()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7135"),
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        [Given("validation endpoint is used")]
        public void GivenValidationEndpointIsUsed()
        {
            _currentEndpoint = "/CardValidation/card/credit/validate";
        }
        
        [Given("the date format {string} is used")]
        public void GivenTheDateFormatIsUsed(string dateFormat)
        {
            _dateFormat = dateFormat;
        }


        [When(@"I post credit card details with {string}, {string}, {string}, {string}")]
        public async Task WhenIPostCreditCardDetailsWith(string owner, string number, string date, string cvv)
        {
            if (_currentEndpoint == null)
            {
                throw new InvalidOperationException("Endpoint is not set. Ensure that the endpoint is defined before making a request.");
            }
            var creditCardValidThruDate = GetValidThruDate(date);
            var card = new CreditCard
            {
                Owner = owner,
                Number = number,
                Date = creditCardValidThruDate,
                Cvv = cvv
            };
            var json = JsonSerializer.Serialize(card);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(_currentEndpoint, content);
            _responseBody = await _response.Content.ReadAsStringAsync();
        }
        
        [When("I post empty message")]
        public async Task WhenIPostEmptyMessage()
        {
            if (_currentEndpoint == null)
            {
                throw new InvalidOperationException("Endpoint is not set. Ensure that the endpoint is defined before making a request.");
            }
            var content = new StringContent(string.Empty, Encoding.UTF8);
            _response = await _client.PostAsync(_currentEndpoint, content);
        }
        
        [When(@"I post card with payload:")]
        public async Task WhenIPostJson(string plainJson)
        {
            if (_currentEndpoint == null)
            {
                throw new InvalidOperationException("Endpoint is not set. Ensure that the endpoint is defined before making a request.");
            }
            var content = new StringContent(plainJson, Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(_currentEndpoint, content);
            _responseBody = await _response.Content.ReadAsStringAsync();
        }



        [Then("the response status should be {int}")]
        public void ThenTheResponseStatusShouldBe(int expectedStatusCode)
        {
            if (_response == null)
            {
                throw new InvalidOperationException("Response is null. Ensure that a request has been made before checking the response.");
            }
            Assert.Equal( expectedStatusCode, (int)_response!.StatusCode);
        }


        [Then("the payment system should be {string}")]
        public void ThenThePaymentSystemShouldBe(string expectedPaymentSystem)
        {
            if (_responseBody == null)
            {
                throw new InvalidOperationException("Response body is null. Ensure that a request has been made before checking the response body.");
            }
            var paymentSystem = GetExpectedPaymentSystem(expectedPaymentSystem);
            Assert.Equal((int)paymentSystem, int.Parse(_responseBody));
        }

        [Then(@"there there should be ""?([^""]*)""? errors? in the answer")]
        public void ThenThereThereShouldBeErrorSInTheAnswer(string errorsCount)
        {
            if (_responseBody == null)
            {
                throw new InvalidOperationException("Response body is null. Ensure that a request has been made before checking the response body.");
            }
            
            _responseJson ??= JsonDocument.Parse(_responseBody).RootElement;
            var actualErrorsCount = _responseJson.GetValueOrDefault().EnumerateObject().Count();
            
            Assert.Equal(int.Parse(errorsCount), actualErrorsCount);

        }

        [Then("error should have {string} and {string}")]
        public void ThenErrorShouldHaveAnd(string errorKey, string errorValue)
        {
            if (_responseBody == null)
            {
                throw new InvalidOperationException("Response body is null. Ensure that a request has been made before checking the response body.");
            }
            _responseJson ??= JsonDocument.Parse(_responseBody).RootElement;
            var errorArray = _responseJson.GetValueOrDefault().GetProperty(errorKey).EnumerateArray();
            Assert.Contains(errorValue, errorArray.Select(e => e.GetString()).ToList());
        }

        [Then("test errors for {string} are as follows:")]
        public void ThenTestErrorsForAreAsFollows(string testCase, DataTable dataTable)
        {
            // find row which has testCase as the column "Test Case"
            if (_responseBody == null)
            {
                throw new InvalidOperationException("Response body is null. Ensure that a request has been made before checking the response body.");
            }
            _responseJson ??= JsonDocument.Parse(_responseBody).RootElement;
            var errors = _responseJson.GetValueOrDefault().EnumerateObject();
            
            var expectedErrors = dataTable.Rows
                .Where(row => row["Test Case"]?.ToString() == testCase)
                .SelectMany(row => row.Keys.Cast<string>()
                    .Where(columnName => columnName != "Test Case" && !string.IsNullOrWhiteSpace(row[columnName]?.ToString()))
                    .Select(columnName => new KeyValuePair<string, string>(columnName, row[columnName]?.ToString())))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            Assert.Equal(expectedErrors.Count, errors.Count());
            
            foreach (var expectedError in expectedErrors)
            {
                if (!errors.Any(e => e.Name == expectedError.Key && e.Value.EnumerateArray().Any(v => v.GetString() == expectedError.Value)))
                {
                    throw new InvalidOperationException($"Expected error '{expectedError.Key}' with value '{expectedError.Value}' not found in response.");
                }
            }
        }
        
        private string GetValidThruDate(string date)
        {
            if (_dateFormat == null)
            {
                _dateFormat = "MM\\/yyyy"; // Default format if not specified
            }
            return date switch
            {
                "<validDate>" => DateTime.UtcNow.AddMonths(2).ToString(_dateFormat),
                "<currentDate>" => DateTime.UtcNow.ToString(_dateFormat),
                "<pastDate>" => DateTime.UtcNow.AddMonths(-1).ToString(_dateFormat),
                _ => date
            };
        }

        private static PaymentSystemType GetExpectedPaymentSystem(string expectedPaymentSystem)
        {
            return expectedPaymentSystem switch
            {
                "VISA" => PaymentSystemType.Visa,
                "MASTERCARD" => PaymentSystemType.MasterCard,
                "AMERICAN_EXPRESS" => PaymentSystemType.AmericanExpress,
                _ => throw new ArgumentException($"Unknown payment system: {expectedPaymentSystem}")
            };
        }
    }
}
