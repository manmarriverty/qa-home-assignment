Feature: Feature2


@tag1
Scenario: Validate Owner field
	Given the user provide card with missing field
	When the user submit the card detail to API
	Then the response shows Owner is required