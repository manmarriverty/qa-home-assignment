using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace CardValidation.IntegrationTests.Hooks
{
    [Binding]
    public class TestSetupHook
    {
        private readonly ScenarioContext _scenarioContext;

        public TestSetupHook(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            _scenarioContext.Add("HttpClient", client);
        }
    }
}