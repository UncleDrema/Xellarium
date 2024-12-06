using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Xellarium.NewClient.Authentication;
using Xellarium.NewClient.Services;

namespace Xellarium.NewClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Configuration.AddEnvironmentVariables();
        
        var clientConfig = builder.Configuration.GetSection("Client").Get<ClientConfig>()!;
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddScoped<ApiMessageHandler>();
        builder.Services.AddHttpClient(ApiAccess.ApiClientName, options =>
        {
            options.BaseAddress = new Uri("http://localhost:5000/api/v2/");
        })
        .AddHttpMessageHandler<ApiMessageHandler>();
        
        builder.Services.AddScoped<TokenAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
        builder.Services.AddScoped<ClientAuthenticationService>();

        builder.Services.AddScoped<IApiAccess, ApiAccess>();

        builder.Services.AddSyncfusionBlazor();

        await builder.Build().RunAsync();
    }
}
