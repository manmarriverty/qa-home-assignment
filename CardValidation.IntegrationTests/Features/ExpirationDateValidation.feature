Feature: Credit Card Expiration Date Validation
  As a payment processor
  I want to validate the expiration date on credit cards
  So that I can reject expired cards before processing payments

  Background:
    Given I have a valid card validation service

  Scenario: Accept card with future expiration date in slash format
    When I validate the expiration date "12/2030"
    Then the validation should succeed
    And the card should be considered valid

  Scenario: Accept card with future expiration date without separator
    When I validate the expiration date "122030"
    Then the validation should succeed

  Scenario: Accept card expiring next month
    When I validate the expiration date for next month
    Then the validation should succeed

  Scenario: Reject card with past expiration date
    When I validate the expiration date "01/2020"
    Then the validation should fail
    And the error message should indicate an invalid date

  Scenario: Reject card with invalid month 00
    When I validate the expiration date "00/2025"
    Then the validation should fail

  Scenario: Reject card with invalid month 13
    When I validate the expiration date "13/2025"
    Then the validation should fail

  Scenario: Reject card with reversed month/year format
    When I validate the expiration date "2025/12"
    Then the validation should fail

  Scenario: Reject card with alphabetic characters
    When I validate the expiration date "AB/2025"
    Then the validation should fail

  Scenario: Reject card with single digit month
    When I validate the expiration date "7/2030"
    Then the validation should fail

  Scenario: Accept card with 2-digit year in future
    When I validate the expiration date "12/99"
    Then the validation should succeed

  Scenario: Reject card with 3-digit year
    When I validate the expiration date "12/203"
    Then the validation should fail

  Scenario: Accept card expiring in December
    When I validate the expiration date "12/2026"
    Then the validation should fail

  Scenario: Validate card through REST API endpoint
    When I submit a credit card with expiration date "06/2030" to the validation endpoint
    Then I should receive a successful response
    And the response should indicate card validation passed
