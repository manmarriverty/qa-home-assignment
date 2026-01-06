using CardValidation.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace CardValidation.IntegrationTests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private static WebApplicationFactory<Program>? _factory;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _factory?.Dispose();
        }

        internal static WebApplicationFactory<Program> GetFactory() => _factory!;
    }
}

