# Card Validation

A small ASP.NET Core Web API that validates credit card details (owner, number, expiration date, CVC) and returns the card's payment system (Visa, MasterCard, American Express) or a set of validation errors.

## Tech Stack

- **.NET 8** - ASP.NET Core Web API
- **Swagger / OpenAPI** - interactive API documentation (Development environment)
- **xUnit** - unit tests
- **Reqnroll** - BDD integration tests (Gherkin)

## Structure

```
CardValidation.sln
├── CardValidation.Core/   # Validation logic and domain types
│   ├── Enums/             # PaymentSystemType
│   └── Services/          # ICardValidationService and CardValidationService
└── CardValidation.Web/    # API host
    ├── Controllers/       # CardValidationController
    ├── Infrustructure/    # Action filter that applies field validation to requests
    └── ViewModels/        # CreditCard request model
```

Test projects (unit and BDD integration tests) are introduced through pull requests against `main`.

## Getting Started

Prerequisite: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet restore
dotnet build
dotnet run --project CardValidation.Web
```

In the Development environment the app serves Swagger UI at `https://localhost:7135/swagger` (HTTP: `http://localhost:5135/swagger`).

## API

### `POST /CardValidation/card/credit/validate`

Request body (JSON):

| Field    | Description                                          | Example               |
| -------- | ---------------------------------------------------- | --------------------- |
| `owner`  | Cardholder name (letters and spaces)                 | `"John Smith"`        |
| `number` | Card number                                          | `"4111111111111111"`  |
| `date`   | Expiration date as `MM/YY`, `MM/YYYY` or `MMYYYY`    | `"12/2030"`           |
| `cvv`    | 3 or 4 digit security code                           | `"123"`               |

```json
{
  "owner": "John Smith",
  "number": "4111111111111111",
  "date": "12/2030",
  "cvv": "123"
}
```

Responses:

- `200 OK` - all fields are valid; the body is the detected payment system (`Visa`, `MasterCard` or `AmericanExpress`).
- `400 Bad Request` - one or more fields are missing or invalid; the body lists errors per field, for example `Date is required` or `Wrong cvv`.

## Validation Rules

| Field  | Rule                                                                                   |
| ------ | -------------------------------------------------------------------------------------- |
| Owner  | One to three words made of Latin letters                                               |
| Number | Matches a supported payment system pattern (Visa, MasterCard, American Express)        |
| Date   | Two-digit month `01`-`12`, followed by a two- or four-digit year, optional `/` between |
| CVC    | Three or four digits                                                                   |

## Running Tests

Once the test projects are present in the solution:

```bash
dotnet test
```

To run a single project, pass its path, for example `dotnet test CardValidation.Core.Tests`.
