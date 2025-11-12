Feature: Credit card validation API
  In order to validate credit cards
  As a client of the API
  I want to receive the correct card type and validation errors

  Scenario: Valid Visa card
    Given I have a valid Visa credit card
    When I send the validation request
    Then the response status code should be 200
    And the payment system type should be Visa

  Scenario: Valid MasterCard card
    Given I have a valid MasterCard credit card
    When I send the validation request
    Then the response status code should be 200
    And the payment system type should be MasterCard

  Scenario: Invalid card with missing Cvv
    Given I have an invalid credit card with missing Cvv
    When I send the validation request
    Then the response status code should be 400
    And the response should contain a Cvv validation message
