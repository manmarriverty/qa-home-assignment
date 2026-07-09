# Card Validation

A small ASP.NET Core API that validates credit card details (owner, number, issue date, CVC) and returns the card type (Visa, MasterCard, American Express) or validation errors.

## Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **xUnit** — unit tests
- **Reqnroll** — BDD integration tests (Gherkin)
- **Moq** — mocking in unit tests
- **Docker** — containerized build and test execution
- **GitHub Actions** — CI pipeline with test results and coverage artifacts

## Structure

```
CardValidation.sln
├── CardValidation.Core/              # Validation logic and domain types
├── CardValidation.Web/               # API, controllers, and request filter
├── CardValidation.Core.Tests/        # Unit tests
├── CardValidation.IntegrationTests/  # Reqnroll API integration tests
├── Dockerfile                        # Multi-stage build (app + tests)
├── docker-compose.yml
└── .github/workflows/ci.yml          # CI pipeline
```
