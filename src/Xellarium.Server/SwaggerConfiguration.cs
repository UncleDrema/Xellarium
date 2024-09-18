using Microsoft.OpenApi.Models;
using Xellarium.Authentication;

namespace Xellarium.Server;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Xellarium.Server", Version = "v1" });
            
            c.AddSecurityDefinition("basic", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });

            c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = Cookies.CookieName,
                Description = $"Cookie authentication using the {Cookies.CookieName} cookie"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }
}