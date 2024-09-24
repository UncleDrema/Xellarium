using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;
using Xunit.Abstractions;

namespace Xellarium.IntegrationTests;

[AllureParentSuite("Intergration tests")]
[AllureSuite("User")]
public class AuthenticationTests
{
    [Fact(DisplayName = "Registration works")]
    public async Task TestRegistrationWorks()
    {
        AllureApi.Step("Connect to test database");
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var (userService, authService) = GetServices(context);
        
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
    
    [Fact(DisplayName = "Registration fails with duplicate name")]
    public async Task TestRegistrationFailsWithDuplicateName()
    {
        AllureApi.Step("Connect to test database");
        await using var context = new TestDatabaseBuilder()
            .WithUsers(new List<User>
            {
                new User
                {
                    Name = "user",
                    PasswordHash = "password"
                }
            })
            .Build();
        
        var (userService, authService) = GetServices(context);
        
        var user = await userService.GetUserByName("user");
        
        AllureApi.Step("Assert exception is thrown");
        await Assert.ThrowsAsync<ArgumentException>(() => authService.RegisterUser("user", "password"));
    }
    
    [Fact(DisplayName = "Authentication works")]
    public async Task TestAuthenticateWorks()
    {
        AllureApi.Step("Connect to test database");
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var (userService, authService) = GetServices(context);
        
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
        await using var context = new TestDatabaseBuilder()
            .Build();
        
        var (userService, authService) = GetServices(context);
        
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
        var userRepository = new UserRepository(context);
        var collectionRepository = new CollectionRepository(context);
        var ruleRepository = new RuleRepository(context);
        var neighborhoodRepository = new NeighborhoodRepository(context);
        var userService = new UserService(userRepository, collectionRepository, ruleRepository, neighborhoodRepository);
        var authService = new AuthenticationService(userService);
        return (userService, authService);
    }
}