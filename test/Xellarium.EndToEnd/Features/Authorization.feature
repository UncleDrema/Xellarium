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
    
  Scenario Outline: Failed login with wrong password
    Given a user exists with username <UserName>, password <Password> and FA secret <Secret>
    When the user logs in with username <UserName>, password <WrongPassword>
    And the user provides the two-factor authentication code from binded 2FA-app with secret <Secret>
    Then the user should not recieve a valid JWT token
  Examples:
    | UserName | Password      | Secret                           | WrongPassword |
    | user     | user          | null                             | wrongpassword |
    | testuser | superpasswrod | NF6YBS4LXPWVYXYJ4RW5RRPETL5BRWMG | wrongpassword |
    
  Scenario Outline: Failed login with wrong 2FA code
    Given a user exists with username <UserName>, password <Password> and FA secret <Secret>
    When the user logs in with username <UserName>, password <Password>
    And the user provides the two-factor authentication code from binded 2FA-app with secret <WrongSecret>
    Then the user should not recieve a valid JWT token
  Examples:
    | UserName | Password      | Secret                           | WrongSecret |
    | testuser | superpasswrod | NF6YBS4LXPWVYXYJ4RW5RRPETL5BRWMG | wrongsecret |
    
  Scenario Outline: Password change
    Given a user exists with username <UserName>, password <Password> and FA secret <Secret>
    When the user logs in with username <UserName>, password <Password>
    And the user provides the two-factor authentication code from binded 2FA-app with secret <Secret>
    And the user changes the password to <NewPassword>
    Then the user should recieve a valid JWT token
  Examples:
    | UserName | Password      | Secret                           | NewPassword |
    | user     | user          | null                             | newpassword |
    | testuser | superpasswrod | NF6YBS4LXPWVYXYJ4RW5RRPETL5BRWMG | newpassword |