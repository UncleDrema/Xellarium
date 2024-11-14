using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Xellarium.Client.Logics;
using Xellarium.Client.Providers;

namespace Xellarium.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddOptions();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlazoredLocalStorage();
        
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        builder.Services.AddScoped<ApiMessageHandler>();
        builder.Services.AddHttpClient("API", options =>
        {
            options.BaseAddress = new Uri("http://localhost:5000/");
        })
        .AddHttpMessageHandler<ApiMessageHandler>();

        builder.Services.AddScoped<IApiLogic, ApiLogic>();

        var app = builder.Build();
        
        
        
        await app.RunAsync();
    }
}