# Credit Card Validation API - QA Automation Assignment

This project is a QA automation test suite built to verify the functionality of a credit card validation API. It uses the BDD framework **Reqnroll (SpecFlow)** with **NUnit** as the test runner. The suite is integrated with **Allure Reports** and supports running inside **Docker** with **CI/CD** integration.

## 🚀 Technologies Used

- .NET 8
- Reqnroll (SpecFlow)
- NUnit
- Allure Report
- WebApplicationFactory (for in-memory API testing)
- Docker
- GitHub Actions
  
## 📁 Project Structure

- CardValidation.Tests 
  - IntegrationTests
   - Features
   - Hooks
   - Steps
  - UnitTests
- /Reports  
  - Allure reports
  - Allure results
- root:
  - .ci.yml
  - dockerfile

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)

### Local Test Execution

1. **Clone the repository**:
   ```bash
   git clone https://github.com/yourusername/qa-home-assignment.git
   cd qa-assignment/CardValidation.Web
   dotnet run
   
2. **Once api service is up and running, follow below commands to execute the script**:
   ```bash
   dotnet clean
   dotnet build
   dotnet test

3. **Generate Allure Report**:
   ```bash
   allure generate ./allure-results --clean -o ./allure-report
   allure open ./allure-report

4. **Code Coverage Report**:
   ```bash
   dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
   reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
   open CoverageReport/index.html

5. **CI Integration**:
   The tests are integrated with GitHub Actions. On every push or PR, tests run in Docker and generate Allure reports.
   ```bash
   📂 .github/workflows/ci.yml defines the CI pipeline.
   
### View Live Reports:

The latest build report is automatically generated and deployed to: **[View Live Report](https://faheem412.github.io/qa-home-assignment/)**
