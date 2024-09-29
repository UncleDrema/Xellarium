using System.Net;
using System.Security.Claims;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.Shared;

namespace Xellarium.Authentication;

public static class Cookies
{
    public const string AuthType = "Cookies";
    public const string CookieName = "UserLoginCookie";

    public static async Task SignInUser(HttpContext ctx, BusinessLogic.Models.User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
    
        var identity = new ClaimsIdentity(claims, AuthType);
    
        await ctx.SignInAsync(AuthType, new ClaimsPrincipal(identity));
    }
}

public static class CookiesExtensions
{
    private static bool IsInRole(this ClaimsPrincipal user, UserRole role)
    {
        return user.IsInRole(role.ToString());
    }
    
    public static void ConfigureCookiesAuthentication(this WebApplicationBuilder builder)
    {
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<AuthenticationFeature>>();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = Cookies.AuthType;
                options.DefaultAuthenticateScheme = Cookies.AuthType;
                options.DefaultSignInScheme = Cookies.AuthType;
                options.DefaultChallengeScheme = Cookies.AuthType;
            })
            .AddCookie(Cookies.AuthType, options =>
            {
                options.Cookie.Name = Cookies.CookieName;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.Admin, policy =>
                policy.RequireAssertion(context =>
                {
                    return true;
                    return context.User.IsInRole(UserRole.Admin);
                }));
            
            options.AddPolicy(Policies.AdminOrOwner, policy =>
                policy.RequireAssertion(context =>
                {
                    return true;
                    logger.LogInformation("Checking {Policy} of user {Id}", Policies.AdminOrOwner, context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (context.User.IsInRole(UserRole.Admin))
                    {
                        logger.LogInformation("User is admin");
                        return true;
                    }
                    
                    if (context.Resource is not DefaultHttpContext ctx)
                    {
                        logger.LogInformation("Resource is not DefaultHttpContext");
                        return false;
                    }
                    var routeData = ctx.GetRouteData();
                    
                    var userId = routeData.Values["id"] as string;
                    return (context.User.IsInRole(UserRole.User) &&
                            context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId));
                }));
            
            options.AddPolicy(Policies.CanAccessCollection, policy =>
                policy.RequireAssertion(async context =>
                {
                    return true;
                    logger.LogInformation("Checking {Policy} of user {Id}", Policies.CanAccessCollection, context.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (context.User.IsInRole(UserRole.Admin))
                        return true;

                    if (context.Resource is not DefaultHttpContext ctx)
                    {
                        logger.LogInformation("Resource is not DefaultHttpContext");
                        return false;
                    }
                    var routeData = ctx.GetRouteData();

                    var userIdRaw = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userIdRaw == null)
                    {
                        logger.LogInformation("User id is null");
                        return false;
                    }
                    var userId = int.Parse(userIdRaw);
                    
                    var collectionIdRaw = routeData.Values["collectionId"] as string;
                    if (collectionIdRaw == null)
                    {
                        logger.LogInformation("Collection id is null");
                        return false;
                    }
                    var collectionId = int.Parse(collectionIdRaw);
                    
                    using var serviceScope = builder.Services.BuildServiceProvider().CreateScope();
                    var collectionService = serviceScope.ServiceProvider.GetRequiredService<ICollectionService>();
                    var collection = await collectionService.GetCollection(collectionId);
                    if (collection == null)
                    {
                        logger.LogInformation("Collection {Id} does not exist", collectionId);
                        return false;
                    }
                    
                    if (collection.IsPrivate && collection.Owner.Id != userId)
                    {
                        return false;
                    }

                    return true;
                }));
        });
    }
}