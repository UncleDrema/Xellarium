using Allure.Net.Commons;
using Allure.Xunit.Attributes;
using Allure.Xunit.Attributes.Steps;
using Xellarium.NewClient.Services;
using Xellarium.Shared;
using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Test;

[AllureParentSuite("Integration tests")]
[AllureSuite("Api Access")]
public class ApiAccessTests
{
    [AllureStep("Login into API as admin")]
    private async Task<IApiAccess> AsAdmin()
    {
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
        Assert.NotEmpty(neighborhoods.Response);
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
        Assert.NotEmpty(collections.Response);
    }
    
    [SkippableFact(DisplayName = "Get collection by id")]
    public async Task TestGetCollectionById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get collection by id");
        
        var collections = await api.GetAvailableCollections();
        var collection = await api.GetCollection(collections.Response!.First().Id);

        Assert.Equal(ResultCode.Ok, collection.Result);
        Assert.NotNull(collection.Response);
    }
    
    [SkippableFact(DisplayName = "Get collection rules by id")]
    public async Task TestGetCollectionRulesById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get collection rules by id");
        
        var collections = await api.GetAvailableCollections();
        var rules = await api.GetCollectionRules(collections.Response!.First().Id);

        Assert.Equal(ResultCode.Ok, rules.Result);
        Assert.NotNull(rules.Response);
        Assert.NotEmpty(rules.Response);
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
        Assert.Equal(UserRole.Admin, user.Response.Role);
    }
    
    [SkippableFact(DisplayName = "Get user by id")]
    public async Task TestGetUserById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get user by id");
        
        var user = await api.GetUser(1);

        Assert.Equal(ResultCode.Ok, user.Result);
        Assert.NotNull(user.Response);
        Assert.Equal(UserRole.Admin, user.Response.Role);
    }
    
    [SkippableFact(DisplayName = "Get neighborhood by id")]
    public async Task TestGetNeighborhoodById()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get neighborhood by id");
        
        var neighborhoods = await api.GetNeighborhoods();
        var neighborhood = await api.GetNeighborhood(neighborhoods.Response!.First().Id);

        Assert.Equal(ResultCode.Ok, neighborhood.Result);
        Assert.NotNull(neighborhood.Response);
    }
    
    [SkippableFact(DisplayName = "Get neighborhood by id with wrong id")]
    public async Task TestGetNeighborhoodByIdFailed()
    {
        var api = await AsAdmin();
        
        Skip.If(Environment.GetEnvironmentVariable("TESTS_STATUS") == "failed");
        AllureApi.Step("Get neighborhood by id with wrong id");
        
        var neighborhood = await api.GetNeighborhood(-1);

        Assert.Equal(ResultCode.Error, neighborhood.Result);
        Assert.Null(neighborhood.Response);
    }
}