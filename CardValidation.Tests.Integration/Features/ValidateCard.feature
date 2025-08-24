Feature: ValidateCard
	The api should validate a credit card and return an error or a payment system code

Background:
	Given validation endpoint is used

# NB!!! This test should pass, remove @ignore as soon as fix is applied
@ignore @card_validation
Scenario: Credit card is valid thru current month
	When I post credit card details with "John Doe", "4242424242424242", "<currentMonth>", "111"
	Then the response status should be 200
	And the payment system should be "VISA"


# We use next data as baseline of 'correct' data. If this test fails, do not trust other tests
# As they use this for a 'nice' path
@smoke @card_validation
Scenario: All credit card data is valid for testing data
	When I post credit card details with "John Doe", "4242424242424242", "<validDate>", "111"
	Then the response status should be 200
	And the payment system should be "VISA"


@card_validation
Scenario Outline: All credit card data is valid
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 200
	And the payment system should be "<payment_system>"
	
	Examples:
		| owner          | number           | date        | cvv  | payment_system   |
		| John Doe       | 4242424242424242 | <validDate> | 111  | VISA             |
		| Amy            | 4012888888881881 | <validDate> | 3214 | VISA             |
		| Some Body      | 371449635398431  | <validDate> | 942  | AMERICAN_EXPRESS |
		| Mark Bob Third | 5105105105105100 | <validDate> | 264  | MASTERCARD       |
		| Tester         | 2221000000000009 | <validDate> | 451  | MASTERCARD       |


@card_validation
Scenario Outline: Correct owner name
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 200 
	And the payment system should be "VISA"
	
	Examples:
		| owner          | number           | date        | cvv |
		| John Doe Third | 4242424242424242 | <validDate> | 111 |
		| JoHn DoE       | 4242424242424242 | <validDate> | 111 |
		| John           | 4242424242424242 | <validDate> | 111 |
		| jOHN dOE       | 4242424242424242 | <validDate> | 111 |


@card_validation
Scenario Outline: Wrong owner name
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And there there should be 1 error in the answer
	And error should have "Owner" and "Wrong owner"
	
	Examples:
		| owner               | number           | date        | cvv |
		| numberinname1       | 4242424242424242 | <validDate> | 111 |
		| Many   Spaces       | 4242424242424242 | <validDate> | 111 |
		| Some Very Long name | 4242424242424242 | <validDate> | 111 |
		| Not Allowed S_mbols | 4242424242424242 | <validDate> | 111 |


# Based on https://docs.stripe.com/testing?testing-method=card-numbers
@card_validation
Scenario Outline: Correct card number
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 200 
	And the payment system should be "<payment_system>"
		Examples:
		| owner    | number           | date        | cvv | payment_system   |
		| John Doe | 4000056655665556 | <validDate> | 111 | VISA             |
		| John Doe | 5555555555554444 | <validDate> | 111 | MASTERCARD       |
		| John Doe | 2223003122003222 | <validDate> | 111 | MASTERCARD       |
		| John Doe | 5105105105105100 | <validDate> | 111 | MASTERCARD       |
		| John Doe | 378282246310005  | <validDate> | 111 | AMERICAN_EXPRESS |
		| John Doe | 371449635398431  | <validDate> | 111 | AMERICAN_EXPRESS |

@card_validation
Scenario Outline: Wrong card numnber
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And there there should be 1 error in the answer
	And error should have "Number" and "Wrong number"
		Examples:
		| owner    | number                           | date        | cvv |
		| John Doe | hello                            | <validDate> | 111 |
		| John Doe | 1                                | <validDate> | 111 |
		| John Doe | 4242 4242 4242 4242              | <validDate> | 111 |
		| John Doe | 4242-4242-4242-4242              | <validDate> | 111 |
		| John Doe | 42424242424242424242424242424242 | <validDate> | 111 |


@card_validation
Scenario Outline: Correct date format
	Given the date format "<date_format>" is used
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 200 
	And the payment system should be "VISA"
		Examples: 
		| owner    | number           | date        | cvv | date_format |
		| John Doe | 4242424242424242 | <validDate> | 111 | MM'/'yyyy   |
		| John Doe | 4242424242424242 | <validDate> | 111 | MM'/'yy     |
		| John Doe | 4242424242424242 | <validDate> | 111 | MMyyyy      |
		| John Doe | 4242424242424242 | <validDate> | 111 | MMyy        |


@card_validation
Scenario Outline: Wrong date format
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And there there should be 1 error in the answer
	And error should have "Date" and "Wrong date"
		Examples:
		| owner    | number           | date       | cvv |
		| John Doe | 4242424242424242 | 123        | 111 |
		| John Doe | 4242424242424242 | 11-11-2021 | 111 |
		| John Doe | 4242424242424242 | 3023       | 111 |
		| John Doe | 4242424242424242 | 302024     | 111 |
		| John Doe | 4242424242424242 | 2A/2025    | 111 |


@card_validation
Scenario: Past date
	When I post credit card details with "John Doe", "4123456789012", "<pastDate>", "111"
	Then the response status should be 400
	And there there should be 1 error in the answer
	And error should have "Date" and "Wrong date"


@card_validation
Scenario Outline: Correct CVV
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 200 
	And the payment system should be "VISA"
		Examples: 
		| owner    | number           | date        | cvv  |
		| John Doe | 4242424242424242 | <validDate> | 123  |
		| John Doe | 4242424242424242 | <validDate> | 1234 |

@card_validation
Scenario Outline: Wrong CVV
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And there there should be 1 error in the answer
	And error should have "Cvv" and "Wrong cvv"
		Examples:
		| owner    | number           | date        | cvv   |
		| John Doe | 4242424242424242 | <validDate> | 1     |
		| John Doe | 4242424242424242 | <validDate> | 50505 |
		| John Doe | 4242424242424242 | <validDate> | 1A    |
		| John Doe | 4242424242424242 | <validDate> | 11!   |
		| John Doe | 4242424242424242 | <validDate> | 11 1  |
		| John Doe | 4242424242424242 | <validDate> | _		|

@card_validation
Scenario Outline: One field is missing
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And there there should be 1 error in the answer
	And error should have "<error_key>" and "<error_value>"
	
	Examples:
		| owner    | number           | date        | cvv | error_key | error_value        |
		|          | 4242424242424242 | <validDate> | 111 | Owner     | Owner is required  |
		| John Doe |                  | <validDate> | 111 | Number    | Number is required |
		| John Doe | 4242424242424242 |             | 111 | Date      | Date is required   |
		| John Doe | 4242424242424242 | <validDate> |     | Cvv       | Cvv is required    |


@card_validation
Scenario Outline: Multiple fields are missing
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And test errors for "<test_case>" are as follows:
		| Test Case | Owner             | Number             | Date             | Cvv             |
		| TC01      | Owner is required |                    |                  | Cvv is required |
		| TC02      |                   | Number is required | Date is required |                 |
		| TC03      |                   | Number is required | Date is required | Cvv is required |
		| TC04      | Owner is required | Number is required | Date is required | Cvv is required |
	
	Examples:
		| owner    | number           | date        | cvv | test_case |
		|          | 4242424242424242 | <validDate> |     | TC01      |
		| John Doe |                  |             | 111 | TC02      |
		| John Doe |                  |             |     | TC03      |
		|          |                  |             |     | TC04      |

@card_validation
Scenario Outline: Multiple invalid fields
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And test errors for "<test_case>" are as follows:
		| Test Case | Owner       | Number       | Date       | Cvv       |
		| TC01      | Wrong owner |              | Wrong date |           |
		| TC02      |             | Wrong number | Wrong date |           |
		| TC03      | Wrong owner | Wrong number |            | Wrong cvv |
		| TC04      | Wrong owner | Wrong number | Wrong date | Wrong cvv |
	
	Examples:
		| owner    | number              | date        | cvv | test_case |
		| John Do3 | 4242424242424242    | <pastDate>  | 111 | TC01      |
		| John Doe | 11                  | <pastDate>  | 111 | TC02      |
		| John_Doe | 4242-4242-4242-4242 | <validDate> | 11! | TC03      |
		| J0hn Doe | 4242 4242 4242 4242 | <pastDate>  | 2   | TC04      |


@card_validation
Scenario Outline: Combination of missing and invalid fields
	When I post credit card details with "<owner>", "<number>", "<date>", "<cvv>"
	Then the response status should be 400 
	And test errors for "<test_case>" are as follows:
		| Test Case | Owner             | Number             | Date             | Cvv             |
		| TC01      | Owner is required |                    | Wrong date       |                 |
		| TC02      |                   | Number is required | Wrong date       | Wrong cvv       |
		| TC03      | Wrong owner       | Wrong number       | Date is required | Cvv is required |
	
	Examples:
		| owner    | number              | date       | cvv | test_case |
		|          | 4242424242424242    | <pastDate> | 111 | TC01      |
		| John Doe |                     | <pastDate> | cvv | TC02      |
		| John_Doe | 4242-4242-4242-4242 |            |     | TC03      |


@card_validation
Scenario: Passing empty message
	When I post empty message
	Then the response status should be 415


@card_validation
Scenario: Passing empty json
	When  I post card with payload:
	"""
	{}
	"""
	Then the response status should be 400
	And there there should be 4 errors in the answer

@card_validation
Scenario: Passing json with missing field
	When I post card with payload:
	"""
	{
		"Owner": "John Doe",
		"Number": "4242424242424242",
		"Cvv": "111"
	}
	"""
	Then the response status should be 400
	And there there should be 1 error in the answer
	And error should have "Date" and "Date is required"