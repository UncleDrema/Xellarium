using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xellarium.NewClient.Services;
using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Test;

public class ApiAccessBuilder
{
    private readonly IConfiguration _configuration;
    private ServiceProvider _serviceProvider;
    private IApiAccess _api;
    
    public ApiAccessBuilder()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
    }
    
    public ApiAccessBuilder ConnectToApi()
    {
        var apiUri = _configuration.GetSection("ApiUri").Value!;
        
        var services = new ServiceCollection();
        services.AddHttpClient(ApiAccess.ApiClientName, options =>
        {
            options.BaseAddress = new Uri(apiUri);
        });
        services.AddScoped<IApiAccess, ApiAccess>();
        
        _serviceProvider = services.BuildServiceProvider();
        _api = _serviceProvider.GetRequiredService<IApiAccess>();
        return this;
    }
    
    public async Task<ApiAccessBuilder> Login()
    {
        var apiUri = _configuration.GetSection("ApiUri").Value!;
        
        var services = new ServiceCollection();
         services.AddHttpClient(ApiAccess.ApiClientName, options =>
        {
            options.BaseAddress = new Uri(apiUri);
        })
        .AddHttpMessageHandler<TestApiMessageHandler>();
        services.AddScoped<TestApiMessageHandler>();
        services.AddScoped<IApiAccess, ApiAccess>();
        
        _serviceProvider = services.BuildServiceProvider();
        _api = _serviceProvider.GetRequiredService<IApiAccess>();

        var defaultUser = _configuration.GetSection("DefaultUser");
        var login = new UserLoginDTO()
        {
            Username = defaultUser["Username"]!,
            Password = defaultUser["Password"]!
        };
        
        var loginResult = await _api.Login(login);
        Assert.Equal(ResultCode.Ok, loginResult.Result);
        Assert.NotNull(loginResult.Response);
        TestApiMessageHandler.Token = loginResult.Response.Token;
        return this;
    }

    public IApiAccess Build()
    {
        return _api;
    }

    private class TestApiMessageHandler : DelegatingHandler
    {
        public static string Token = string.Empty;
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}