# Home Assignment

You will be required to write unit tests and automated tests for a payment application to demonstrate your skills. 

## Application information 

It’s a small microservice that validates provided Credit Card data and returns either an error or type of credit card application. 

## API Requirements 

API that validates credit card data.

**Input parameters:** Card owner, Credit Card number, issue date and CVC. 

**Validation logic:**
- All fields must be provided
- Card owner must not have existing credit card info
- Credit card must not be expired
- Card number must match the type pattern
- CVC must be valid for the card type

**Output:**
- Returns the credit card type (`MasterCard`, `Visa`, or `American Express`) if valid
- Returns validation errors if invalid

## Technical Requirements 

- Unit tests covering **80%+** of the application  
- Integration tests using **Reqnroll**  
- Bonus: **Dockerized pipeline** for automated testing and coverage report generation  
- Bonus: **Test execution reports** (HTML coverage + TRX results)

## Running the Application 

1. Fork the repository  
2. Clone the repository on your local machine  
3. Compile and run using **Visual Studio 2022**

---

# Automated Tests & Coverage

This project includes:
- Unit tests (Core logic)
- Integration tests (API layer)
- Reqnroll BDD scenarios
- Automated Docker setup for coverage + TRX generation

---

## Run Tests via Docker

### ️Build the Docker test image
```bash
docker build -t cardvalidation-tests -f Dockerfile.tests .
```
### Run tests and extract artifacts
```bash
# Create container
docker create --name cvtests cardvalidation-tests

# Copy out artifacts (coverage reports and TRX results)
docker cp cvtests:/artifacts ./artifacts

# Cleanup
docker rm cvtests
```
### View the coverage report
```bash
After running the commands above, open:

Windows: artifacts\coverage-report\index.html

TRX test logs are available in:
artifacts/trx/
```


