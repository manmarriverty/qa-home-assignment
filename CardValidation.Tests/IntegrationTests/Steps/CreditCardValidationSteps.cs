using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Reqnroll;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace CardValidation.Tests.Steps
{
    [Binding]
    [AllureNUnit]
    [AllureSuite("Credit Card Validation API Tests")]
    public class CreditCardValidationSteps
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private HttpRequestMessage? _request;
        private HttpResponseMessage? _response;
        private bool _disposed = false;
        private bool _cardTypeAlreadyLogged = false; // Flag to prevent duplicate logging
        private const string ValidationEndpoint = "/CardValidation/card/credit/validate";

        // Card type mapping
        private static readonly Dictionary<string, string> _cardTypeMapping = new Dictionary<string, string>
        {
            { "10", "Visa" },
            { "20", "MasterCard" },
            { "30", "American Express" }
        };

        public CreditCardValidationSteps()
        {
            // Create the test server factory
            _factory = new WebApplicationFactory<Program>();

            // Create HTTP client from the test server
            _client = _factory.CreateClient();

            Console.WriteLine("TestHost initialized for API testing");
        }

        [Given(@"I prepare a credit card with:")]
        [AllureStep("Prepare credit card with provided data")]
        public void GivenIPrepareACreditCardWith(Table table)
        {
            if (table.Rows.Count == 0)
                throw new InvalidOperationException("Table must contain at least one data row");

            var row = table.Rows[0];

            var owner = row["Owner"]?.Trim() ?? string.Empty;
            var number = row["Number"]?.Trim() ?? string.Empty;
            var cvv = row["Cvv"]?.Trim() ?? string.Empty;
            var issueDate = row["IssueDate"]?.Trim() ?? string.Empty;

            // Reset the flag for new test
            _cardTypeAlreadyLogged = false;

            // Create the request payload
            var payload = new
            {
                owner,
                number,
                cvv,
                date = issueDate
            };

            var jsonContent = JsonSerializer.Serialize(payload);

            _request = new HttpRequestMessage(HttpMethod.Post, ValidationEndpoint)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            // Add card details to Allure report
            AllureApi.AddAttachment("Request Payload", "application/json", Encoding.UTF8.GetBytes(jsonContent));
        }

        [When(@"I send the card to the validation API")]
        [AllureStep("Send credit card data to API")]
        public async Task WhenISendTheCardToTheValidationAPI()
        {
            if (_request == null)
                throw new InvalidOperationException("Request has not been initialized.");

            _response = await _client.SendAsync(_request);

            // Add response details to Allure report
            var responseContent = await _response.Content.ReadAsStringAsync();
            AllureApi.AddAttachment("API Response", "application/json", Encoding.UTF8.GetBytes(responseContent));
        }

        [Then(@"the response status should be (.*)")]
        [AllureStep("Validate HTTP response status")]
        public void ThenTheResponseStatusShouldBe(int expectedStatus)
        {
            if (_response == null)
                throw new InvalidOperationException("Response has not been received yet.");

            var actualStatus = (int)_response.StatusCode;
            var responseBody = _response.Content.ReadAsStringAsync().Result;

            // Add status validation to Allure report
            AllureApi.AddAttachment("Status Validation", "text/plain", 
                Encoding.UTF8.GetBytes($"Expected: {expectedStatus}, Actual: {actualStatus}"));

            Assert.That(actualStatus, Is.EqualTo(expectedStatus),
                $"Expected HTTP {expectedStatus}, but got {actualStatus}. Response body: {responseBody}");
        }

        [Then(@"the response body should be ""(.*)""")]
        [AllureStep("Validate response body content")]
        public async Task ThenTheResponseBodyShouldBe(string expectedResult)
        {
            if (_response == null)
                throw new InvalidOperationException("Response has not been received yet.");

            var responseContent = await _response.Content.ReadAsStringAsync();
            
            // Clean the expected result and response content
            var cleanExpectedResult = expectedResult?.Trim() ?? string.Empty;
            var cleanResponseContent = responseContent?.Trim() ?? string.Empty;
            
            // Only log card type once per test scenario
            if (_response.IsSuccessStatusCode && 
                _cardTypeMapping.ContainsKey(cleanExpectedResult) && 
                !_cardTypeAlreadyLogged)
            {
                var cardType = _cardTypeMapping[cleanExpectedResult];
                var cardTypeInfo = $"Card Type: {cardType} (Code: {cleanExpectedResult})";
                
                Console.WriteLine($"✓ CARD TYPE DETECTED: {cardTypeInfo}");
                
                try
                {
                    // Add card type information to Allure report
                    AllureApi.AddAttachment("Card Type Detected", "text/plain", 
                        Encoding.UTF8.GetBytes(cardTypeInfo));
                    
                    // Add as parameters to the current test step
                    AllureApi.AddTestParameter("Card Type", cardType);
                    AllureApi.AddTestParameter("Response Code", cleanExpectedResult);
                    
                    _cardTypeAlreadyLogged = true; // Set flag to prevent duplicate logging
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR adding to Allure: {ex.Message}");
                }
            }
            
            // Add response validation details only if not already added
            if (!_cardTypeAlreadyLogged || !_response.IsSuccessStatusCode)
            {
                var validationInfo = $"Expected: '{cleanExpectedResult}'\nActual: '{cleanResponseContent}'";
                try
                {
                    AllureApi.AddAttachment("Response Validation", "text/plain", 
                        Encoding.UTF8.GetBytes(validationInfo));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR adding response validation: {ex.Message}");
                }
            }
            
            Assert.That(cleanResponseContent, Does.Contain(cleanExpectedResult).IgnoreCase,
                $"Expected response body to contain '{cleanExpectedResult}', but got: '{cleanResponseContent}'");
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _request?.Dispose();
                _response?.Dispose();
                _client?.Dispose();
                _factory?.Dispose();
                _disposed = true;
            }
        }
    }
}