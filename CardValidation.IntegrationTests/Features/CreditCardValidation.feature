Feature: Credit Card Validation
    As a system user
    I want to validate credit card information
    So that I can ensure the card data is correct

    Scenario: Validate a valid Visa credit card
        Given I have a credit card with the following data:
            | Owner      | Number             | Date  | Cvv |
            | John Smith | 4111111111111111   | 12/25 | 123 |
        When I send the validation request
        Then I should receive status code 200
        And the card type should be "Visa"

    Scenario: Validate a valid MasterCard credit card
        Given I have a credit card with the following data:
            | Owner        | Number             | Date  | Cvv |
            | Mary Johnson | 5412345678901234   | 12/25 | 123 |
        When I send the validation request
        Then I should receive status code 200
        And the card type should be "MasterCard"

    Scenario: Validate a credit card with invalid data
        Given I have a credit card with the following data:
            | Owner    | Number           | Date  | Cvv |
            | John123  | 1234567890123456 | 13/25 | abc |
        When I send the validation request
        Then I should receive status code 400

    Scenario: Missing owner causes validation error
        Given I have a credit card with the following data:
            | Owner | Number           | Date  | Cvv |
            |       | 4111111111111111 | 12/99 | 123 |
        When I send the validation request
        Then I should receive status code 400