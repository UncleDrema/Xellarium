using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Xellarium.Shared;

namespace Xellarium.Authentication;

public static class Jwt
{
    public static string AuthType => JwtBearerDefaults.AuthenticationScheme;
}

public static class JwtExtensions
{
    private static bool IsInRole(this ClaimsPrincipal user, UserRole role)
    {
        return user.IsInRole(role.ToString());
    }
    
    public static void ConfigureJwtAuthentication(this WebApplicationBuilder builder)
    {
        var sp = builder.Services.BuildServiceProvider();
        var jwtConfig = sp.GetRequiredService<JwtAuthorizationConfiguration>();
        builder.Services.AddAuthentication(Jwt.AuthType)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey))
                };
            });

        
        var logger = sp.GetRequiredService<ILogger<AuthenticationFeature>>();

        bool MakeAssertionOnUser(AuthorizationHandlerContext context, Func<ClaimsPrincipal?, bool> assertion)
        {
            if (context.Resource is not DefaultHttpContext httpContext)
            {
                logger.LogError("Resource is not DefaultHttpContext");
                return false;
            }
            
            if (!httpContext.Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                logger.LogError("Authorization header is missing");
                return false;
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey))
            };

            var token = httpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "").Replace("bearer ", "");
            
            var user = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return assertion(user);
        }
        
        builder.Services.AddAuthorization(options =>
        {
            void AddPolicy(string policyName, Func<ClaimsPrincipal?, bool> assertion)
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAssertion(context => MakeAssertionOnUser(context, assertion));
                });
            }
            
            options.DefaultPolicy = new AuthorizationPolicyBuilder(Jwt.AuthType).RequireAssertion(
                context => MakeAssertionOnUser(context, user => user is not null)).Build();
            
            AddPolicy(Policies.Admin, user =>
            {
                if (user is null)
                {
                    return false;
                }

                return user.IsInRole(UserRole.Admin);
            });
        });
    }
}