using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;
using Xellarium.IntegrationTests;
using Xellarium.NewClient.Services;
using Xellarium.Shared;
using Xellarium.Shared.DTO;
using Xunit.Abstractions;
using IAuthenticationService = Xellarium.BusinessLogic.Services.IAuthenticationService;
using ICollectionService = Xellarium.BusinessLogic.Services.ICollectionService;
using INeighborhoodService = Xellarium.BusinessLogic.Services.INeighborhoodService;
using IRuleService = Xellarium.BusinessLogic.Services.IRuleService;
using IUserService = Xellarium.BusinessLogic.Services.IUserService;

namespace Xellarium.NewClient.Test;

[AllureParentSuite("Integration tests")]
[AllureSuite("Api Access")]
public class ApiAccessTests
{
    private readonly DatabaseFixture _databaseFixture;
    
    [AllureBefore("Connect to database")]
    public ApiAccessTests(ITestOutputHelper testOutputHelper)
    {
        _databaseFixture = new DatabaseFixture(testOutputHelper);
    }
    
    [AllureStep("Login into API as admin")]
    private async Task<IApiAccess> AsAdmin()
    {
        await _databaseFixture.EnsureAdminExists();
        return (await new ApiAccessBuilder().ConnectToApi().Login()).Build();
    }
    
    [AllureStep("Do not login into API")]
    private async Task<IApiAccess> AsAnonymous()
    {
        return new ApiAccessBuilder().ConnectToApi().Build();
    }
    
    [SkippableFact(DisplayName = "Login into API")]
    public async Task TestLogin()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Login into API");
        
        var login = new UserLoginDTO()
        {
            Username = "admin",
            Password = "admin"
        };
        
        var loginResult = await api.Login(login);
        
        Assert.Equal(ResultCode.Ok, loginResult.Result);
        Assert.NotNull(loginResult.Response);
        Assert.NotNull(loginResult.Response.Token);
    }
    
    [SkippableFact(DisplayName = "Login into API with wrong credentials")]
    public async Task TestLoginFailed()
    {
        var api = await AsAnonymous();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Login into API with wrong credentials");
        
        var login = new UserLoginDTO()
        {
            Username = "admin",
            Password = "wrong"
        };
        
        var loginResult = await api.Login(login);
        
        Assert.Equal(ResultCode.Unauthorized, loginResult.Result);
        Assert.Null(loginResult.Response);
    }
    
    [SkippableFact(DisplayName = "Get all neighborhoods")]
    public async Task TestGetAllNeighborhoods()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get all neighborhoods");
        
        var neighborhoods = await api.GetNeighborhoods();

        Assert.Equal(ResultCode.Ok, neighborhoods.Result);
        Assert.NotNull(neighborhoods.Response);
    }
    
    [SkippableFact(DisplayName = "Get all collections")]
    public async Task TestGetAllCollections()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get all collections");
        
        var collections = await api.GetAvailableCollections();

        Assert.Equal(ResultCode.Ok, collections.Result);
        Assert.NotNull(collections.Response);
    }
    
    [SkippableFact(DisplayName = "Get rule by id")]
    public async Task TestGetRuleById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get rule by id");

        var (users, auth, cols, rules, neighborhoods) = GetServices(_databaseFixture.Context);
        var n = new Neighborhood()
        {
            Name = "Test Neighborhood",
            Offsets = new List<Vec2>()
        };
        await neighborhoods.AddNeighborhood(n);
        var rule = new Rule()
        {
            Name = "TestRule",
            Owner = (await users.GetUsers()).First(),
            GenericRule = GenericRule.GameOfLife,
            Neighborhood = n
        };
        await rules.AddRule(rule);
        var collection = await api.GetRule(rule.Id);

        Assert.Equal(ResultCode.Ok, collection.Result);
        Assert.NotNull(collection.Response);
    }
    
    [SkippableFact(DisplayName = "Get collection by id")]
    public async Task TestGetCollectionById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get collection by id");

        var (users, auth, cols, rules, neighborhoods) = GetServices(_databaseFixture.Context);
        var col = new Collection()
        {
            Name = "TestCollections",
            Owner = (await users.GetUsers()).First()
        };
        await cols.AddCollection(col);
        var collection = await api.GetCollection(col.Id);

        Assert.Equal(ResultCode.Ok, collection.Result);
        Assert.NotNull(collection.Response);
    }
    
    [SkippableFact(DisplayName = "Get collection rules by id")]
    public async Task TestGetCollectionRulesById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get collection rules by id");
        
        var (users, auth, cols, rules, neighborhoods) = GetServices(_databaseFixture.Context);
        var col = new Collection()
        {
            Name = "TestCollections",
            Owner = (await users.GetUsers()).First()
        };
        await cols.AddCollection(col);
        var colRules = await api.GetCollectionRules(col.Id);

        Assert.Equal(ResultCode.Ok, colRules.Result);
        Assert.NotNull(colRules.Response);
    }
    
    [SkippableFact(DisplayName = "Get current user")]
    public async Task TestGetCurrentUser()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get current user");
        
        var user = await api.GetCurrentUser();

        Assert.Equal(ResultCode.Ok, user.Result);
        Assert.NotNull(user.Response);
    }
    
    [SkippableFact(DisplayName = "Get user by id")]
    public async Task TestGetUserById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get user by id");
        
        var (users, auth, cols, rules, neighborhoods) = GetServices(_databaseFixture.Context);

        var user = await api.GetUser((await users.GetUsers()).First().Id);

        Assert.Equal(ResultCode.Ok, user.Result);
        Assert.NotNull(user.Response);
    }
    
    [SkippableFact(DisplayName = "Get neighborhood by id")]
    public async Task TestGetNeighborhoodById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get neighborhood by id");
        
        var (users, auth, cols, rules, neighborhoods) = GetServices(_databaseFixture.Context);
        var n = new Neighborhood()
        {
            Name = "Test Neighborhood",
            Offsets = new List<Vec2>()
        };
        await neighborhoods.AddNeighborhood(n);
        var neighborhood = await api.GetNeighborhood(n.Id);

        Assert.Equal(ResultCode.Ok, neighborhood.Result);
        Assert.NotNull(neighborhood.Response);
    }
    
    [AllureStep("Create user and authentication serivces")]
    private (IUserService, IAuthenticationService, ICollectionService, IRuleService, INeighborhoodService) GetServices(XellariumContext context)
    {
        var logger = new LoggerFactory().CreateLogger<UnitOfWork>();
        var unitOfWork = new UnitOfWork(context, logger);
        var userService = new UserService(unitOfWork, new LoggerFactory().CreateLogger<UserService>());
        var authService = new AuthenticationService(userService);
        var colService = new CollectionService(unitOfWork);
        var ruleService = new RuleService(unitOfWork);
        var neighborhoodService = new NeighborhoodService(unitOfWork);
        return (userService, authService, colService, ruleService, neighborhoodService);
    }
}