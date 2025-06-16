using Allure.Net.Commons;
using Reqnroll;
using System.Linq;
using System.Collections;

namespace Hooks.Hooks 
{
    [Binding]
    public class AllureNamingHooks
    {
        private readonly ScenarioContext _scenarioContext;

        // Constructor for dependency injection of ScenarioContext
        public AllureNamingHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        /// <summary>
        /// This hook runs before each scenario execution.
        /// It dynamically sets the Allure test case name for Scenario Outlines
        /// using the 'TestCaseName' column from the Examples table.
        /// </summary>
        /// <param name="scenarioContext">Provides information about the current scenario.</param>
        [BeforeScenario(Order = 1)] // Order ensures this hook runs very early
        public void SetUniqueAllureTestCaseName()
        {
            // ScenarioInfo.Arguments is of type IOrderedDictionary.
            // To use LINQ methods like Any() and ToDictionary(), we need to cast its elements.
            // We use Cast<DictionaryEntry>() to treat each item as a key-value pair.
            var scenarioArguments = _scenarioContext.ScenarioInfo.Arguments.Cast<DictionaryEntry>();

            // Check if the current scenario is an example from a Scenario Outline by checking if it has arguments
            if (scenarioArguments.Any())
            {
                // Convert the arguments to a dictionary for easier access by parameter name (string key, object value)
                var parameters = scenarioArguments.ToDictionary(
                    entry => (string)entry.Key, // Explicitly cast key to string
                    entry => entry.Value // Value can be an object
                );

                string? testCaseName = null; // Initialize to null to resolve potential "not assigned" warnings

                // Try to get the unique identifier from the "TestCaseName" column
                // This corresponds to the 'TestCaseName' column in your feature file's Examples table.
                if (parameters.TryGetValue("TestCaseName", out object testCaseNameObj) && testCaseNameObj is string tcNameString)
                {
                    testCaseName = tcNameString; // Assign the value if successfully retrieved and cast
                }

                if (!string.IsNullOrEmpty(testCaseName))
                {
                    // Construct the unique test name.
                    // This will result in names like "Validate Credit Card - Valid Visa Card"
                    AllureApi.SetTestName($"{_scenarioContext.ScenarioInfo.Title} - {testCaseName}");
                }
                else
                {
                    // Fallback: If 'TestCaseName' is missing or not a string,
                    // use the first argument's value or just the scenario title.
                    // This ensures a name is always set, even if configuration is incomplete.
                    string fallbackIdentifier = parameters.Any() ? parameters.First().Value.ToString() : "NoSpecificIdentifier";
                    AllureApi.SetTestName($"{_scenarioContext.ScenarioInfo.Title} - {fallbackIdentifier}");
                    System.Console.WriteLine($"Warning: 'TestCaseName' not found or invalid. Using fallback for scenario: {_scenarioContext.ScenarioInfo.Title}");
                }
            }
            else
            {
                // For regular (non-outline) scenarios, just use the scenario title as is
                AllureApi.SetTestName(_scenarioContext.ScenarioInfo.Title);
            }
        }
    }
}

