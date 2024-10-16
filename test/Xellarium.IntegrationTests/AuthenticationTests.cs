using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;

namespace Xellarium.IntegrationTests;

[AllureParentSuite("Intergration tests")]
[AllureSuite("User")]
public class AuthenticationTests : IDisposable
{
    private readonly DatabaseFixture _databaseFixture;
    
    [AllureBefore("Connect to database")]
    public AuthenticationTests()
    {
        _databaseFixture = new DatabaseFixture();
    }
    
    [Fact(DisplayName = "Registration works")]
    public async Task TestRegistrationWorks()
    {
        AllureApi.Step("Connect to test database");
        
        var (userService, authService) = GetServices(_databaseFixture.Context);
        
        AllureApi.Step("Register user");
        var user = await authService.RegisterUser("user", "password");
        
        AllureApi.Step("Assert user is created");
        Assert.NotNull(user);
        
        AllureApi.Step("Assert user is in database");
        var allUsers = (await userService.GetUsers()).ToList();
        
        AllureApi.Step("Assert use name is correct");
        Assert.Single(allUsers);
        Assert.Equal("user", allUsers.First().Name);
        
        AllureApi.Step("Assert user can be retrieved by id");
        var userById = await userService.GetUser(user.Id);
        
        Assert.NotNull(userById);
        Assert.Equal("user", userById.Name);
    }
    
    [Fact(DisplayName = "Authentication works")]
    public async Task TestAuthenticateWorks()
    {
        AllureApi.Step("Connect to test database");
        
        var (userService, authService) = GetServices(_databaseFixture.Context);
        
        AllureApi.Step("Register user");
        await authService.RegisterUser("user", "password");
        
        AllureApi.Step("Try authenticate user");
        var user = await authService.AuthenticateUser("user", "password");
        
        AllureApi.Step("Assert user is authenticated and retrieved correctly");
        Assert.NotNull(user);
        Assert.Equal("user", user.Name);
    }
    
    [Fact(DisplayName = "Authentication fails with wrong password")]
    public async Task TestAuthenticateFailsWithWrongPassword()
    {
        AllureApi.Step("Connect to test database");
        
        var (userService, authService) = GetServices(_databaseFixture.Context);
        
        AllureApi.Step("Register user");
        await authService.RegisterUser("user", "password");
        
        AllureApi.Step("Try authenticate user with wrong password");
        var user = await authService.AuthenticateUser("user", "wrongpassword");
        
        AllureApi.Step("Assert user is not authenticated");
        Assert.Null(user);
    }

    [AllureStep("Create user and authentication serivces")]
    private (IUserService, IAuthenticationService) GetServices(XellariumContext context)
    {
        var logger = new LoggerFactory().CreateLogger<UnitOfWork>();
        var unitOfWork = new UnitOfWork(context, logger);
        var userService = new UserService(unitOfWork, new LoggerFactory().CreateLogger<UserService>());
        var authService = new AuthenticationService(userService);
        return (userService, authService);
    }
    
    [AllureAfter("Clear database")]
    public void Dispose()
    {
        _databaseFixture.Dispose();
    }
}