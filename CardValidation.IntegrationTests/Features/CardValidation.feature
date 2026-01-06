Feature: Credit Card Validation
	As a payment system
	I want to validate credit card information
	So that I can process payments securely

Background:
	Given the API is available

Scenario: Valid Visa card validation
	Given I have a valid Visa card with the following details:
		| Owner | Number              | Date   | Cvv |
		| John Doe | 4111111111111111 | 12/2020 | 123 |
	When I submit the card for validation
	Then the validation should succeed
	And the response should contain payment system type "Visa"

Scenario: Valid MasterCard validation
	Given I have a valid MasterCard with the following details:
		| Owner | Number              | Date   | Cvv |
		| Jane Smith | 5555555555554444 | 06/2020 | 456 |
	When I submit the card for validation
	Then the validation should succeed
	And the response should contain payment system type "MasterCard"

Scenario: Valid American Express validation
	Given I have a valid American Express card with the following details:
		| Owner | Number           | Date   | Cvv  |
		| Bob Johnson | 378282246310005 | 12/2020 | 1234 |
	When I submit the card for validation
	Then the validation should succeed
	And the response should contain payment system type "AmericanExpress"

Scenario: Missing owner name
	Given I have a card with missing owner:
		| Number              | Date   | Cvv |
		| 4111111111111111 | 12/2020 | 123 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain error "Owner is required"

Scenario: Invalid card number
	Given I have a card with invalid number:
		| Owner | Number              | Date   | Cvv |
		| John Doe | 1234567890123456 | 12/2020 | 123 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain error "Wrong number"

Scenario: Invalid date format
	Given I have a card with invalid date:
		| Owner | Number              | Date   | Cvv |
		| John Doe | 4111111111111111 | 13/2020 | 123 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain error "Wrong date"

Scenario: Invalid CVC
	Given I have a card with invalid CVC:
		| Owner | Number              | Date   | Cvv |
		| John Doe | 4111111111111111 | 12/2020 | 12 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain error "Wrong cvv"

Scenario: Multiple validation errors
	Given I have a card with multiple invalid fields:
		| Owner | Number              | Date   | Cvv |
		| John123 | 1234567890123456 | 13/2020 | 12 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain multiple errors

Scenario: Invalid date format in feature file
	Given I have a card with invalid date format:
		| Owner | Number              | Date    | Cvv |
		| John Doe | 4111111111111111 | 2025-12 | 123 |
	When I submit the card for validation
	Then the validation should fail
	And the response should contain error "Wrong date"

