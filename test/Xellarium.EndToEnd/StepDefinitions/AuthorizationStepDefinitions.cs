using System;
using OtpNet;
using Reqnroll;
using Reqnroll.UnitTestProvider;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared;
using Xellarium.Shared.DTO;
using Xunit;

namespace Xellarium.EndToEnd.StepDefinitions;

[Binding]
public sealed class AuthorizationStepDefinitions(IReqnrollOutputHelper output, IApiLogic logic,
    IUnitTestRuntimeProvider unitTestRuntimeProvider, IAuthenticationService authService)
{
    private string? _loginUsername;
    private string? _loginPassword;
    private string? _loginCode;
    
    [Given(@"a user exists with username (.*), password (.*) and FA secret (.*)")]
    public void GivenAUserExistsWithUsernameAdminPasswordAdminAndFaSecretNull(string username, string password, string? secret)
    {
        if (Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed")
            unitTestRuntimeProvider.TestIgnore("Skipped because previous tests failed");
        
        output.WriteLine($"Given user {username} with password {password} and secret {secret} (is null: {secret is null})");
        authService.RegisterUser(username, password, secret);
    }

    [When(@"the user logs in with username (.*), password (.*)")]
    public void WhenTheUserLogsInWithUsernameAdminPasswordAdmin(string username, string password)
    {
        output.WriteLine($"When user {username} with password {password} logs in");
        _loginUsername = username;
        _loginPassword = password;
    }

    [When(@"the user provides the two-factor authentication code from binded 2FA-app with secret (.*)")]
    public void WhenTheUserProvidesTheTwoFactorAuthenticationCodeNull(string? secret)
    {
        string? code = null;
        if (secret is not null)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secret));
            code = totp.ComputeTotp();
        }
        _loginCode = code;
    }

    [Then(@"the user should recieve a valid JWT token")]
    public void ThenTheUserShouldRecieveAValidJwtToken()
    {
        Assert.NotNull(_loginUsername);
        Assert.NotNull(_loginPassword);

        var loginSuccessful = logic.IsLoginSuccessful(new UserLoginDTO
        {
            Username = _loginUsername,
            Password = _loginPassword,
            TwoFactorCode = _loginCode
        });
        Assert.True(loginSuccessful);
    }
}