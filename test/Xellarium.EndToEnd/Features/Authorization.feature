@allure.parentSuite:EndToEnd
@allure.suite:Authorization
Feature: User Authorization
  Scenario Outline: Successful login
    Given a user exists with username <UserName>, password <Password> and FA secret <Secret>
    When the user logs in with username <UserName>, password <Password>
    And the user provides the two-factor authentication code from binded 2FA-app with secret <Secret>
    Then the user should recieve a valid JWT token
  Examples:
    | UserName | Password      | Secret                           |
    | user     | user          | null                             |
    | testuser | superpasswrod | NF6YBS4LXPWVYXYJ4RW5RRPETL5BRWMG |