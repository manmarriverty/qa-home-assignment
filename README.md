# Unit tests

For a given project unit testing is required for this class:
- CardValidation.Core/Services/CardValidationService.cs - full cover with unit tests
- CardValidation.Web/Infrustructure/CreditCardValidationFilter.cs - only behavior of filter
- CardValidation.Web/Controllers/CardValidationController.cs - only behavior of controller

Files not to be tested:
- CardValidation.Core/Enums/PaymentSystemType.cs - no logic
- CardValidation.Web/ViewModels/CreditCard.cs - no logic
- CardValidation.Web/Program.cs - bad practice to test Program.cs, no use

Achieved coverage is 91% for given project, which is above the 80% requirement.
Most of missing coverage comes from Program.cs, which is not a subject to unit testing.

# Found issues

## CardValidationService.cs

### ValidateIssueDate method in CardValidationService.cs

- Method seems to be checking for the **expiry** or **valid thru** date, but method is called ValidateIssueDate which suggests it validates the **issue** date.
I would suggest renaming the method to `ValidateExpiryDate`, `ValidateValidThruDate` or similar to avoid confusion.

- **CRITICAL!** In case the above statement is correct, below is the most important thing - most likely a **bug** and to be **urgently reported to development team!**
```csharp
var issueDateTime = new DateTime(year / 1000 > 0 ? year : 2000 + year, month, 1);
return DateTime.UtcNow < issueDateTime;
```
Possible fix:
```csharp
var expiryDate = new DateTime(year / 1000 > 0 ? year : 2000 + year, month, 1).AddMonths(1);
return DateTime.UtcNow < expiryDate;
```
We are checking against a first day of a month for expiry date, but it should be the first day of the next month.
Cards usually have "valid thru" date, which means the card is valid until the end of the month, not the beginning.
*For example, if it says 12/26, the debit card will expire on the last day of December 2026 (i.e., December 31st, 2026).* (https://fi.money/guides/debit-cards/debit-card-expiry-reasons-for-expiry-renewal-process-online)
Test `ValidateIssueDate_CurrentMonth_ReturnsTrue` is currently skipped, but must be enabled if a bug is actually present after the fix is applied.

Also worth mentioning that valid thru date is not connected to UTC time or basically any timezone, it is a expiry date and can differ based on issuing country or institution.
So a person making a transaction in last hours of his card's validity may encounter an issue with current validation logic. I advice talking with business analytics regarding this issue and maybe adjust the code.
Maybe add an error to client if trying to make a transaction on last day of card's validity, or try to let the transaction go through, and if it fails, return an error to client.

- Should be careful if YY format is still supported after year 2100, code will break (very similar to https://en.wikipedia.org/wiki/Year_2000_problem)
Problem comes from `2000 + year` part in this line:
```csharp
var issueDateTime = new DateTime(year / 1000 > 0 ? year : 2000 + year, month, 1);
```

- If possible, ask the development team to use IDateTimeProvider abstraction to allow injecting needed datetime in tests (see https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
*Handle stub static references with seams* reference).
This will help with tests connected to date validation as currently date calculation logic is handled on a test side, which is not considered a best practice:
```csharp
var nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MM\\/yyyy");
```
Overall, nice to have, but not critical. Also in case IDateTimeProvider is added, could use `[Theory]` instead of `[Fact]` for this method tests to cover more cases via single test.


### ValidateOwner method in CardValidationService.cs

- Validation has inconsistency regarding trailing spaces, because from this part `+ ?` in a pattern a single trailing space is allowed
```csharp
^((?:[A-Za-z]+ ?){1,3})$
```
However the single leading space or multiple trailing spaces are not allowed. Probably not a major issue, but might be worth discussing with the team.

### ValidateNumber method in CardValidationService.cs

- All validation checks (against Visa, Mastercard and American Express) are for regex format only, consider adding a check for Luhn validity on backend https://en.wikipedia.org/wiki/Luhn_algorithm.
This would lower the load on card processing service, when incorrect card number lands on it.

## Misc

- Typo in Infrustructure folder name, should be Infrastructure.

# Home Assignment

You will be required to write unit tests and automated tests for a payment application to demonstrate your skills. 

# Application information 

It’s an small microservice that validates provided Credit Card data and returns either an error or type of credit card application. 

# API Requirements 

API that validates credit card data. 

Input parameters: Card owner, Credit Card number, issue date and CVC. 

Logic should verify that all fields are provided, card owner does not have credit card information, credit card is not expired, number is valid for specified credit card type, CVC is valid for specified credit card type. 

API should return credit card type in case of success: Master Card, Visa or American Express. 

API should return all validation errors in case of failure. 


# Technical Requirements 

 - Write unit tests that covers 80% of application 
 - Write integration tests (preferably using Reqnroll framework) 
 - As a bonus: 
    - Create a pipeline where unit tests and integration tests are running with help of Docker. 
    - Produce tests execution results. 

# Running the  application 

1. Fork the repository
2. Clone the repository on your local machine 
3. Compile and Run application Visual Studio 2022.
