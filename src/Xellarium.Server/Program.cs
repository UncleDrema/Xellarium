using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Xellarium.Authentication;
using Xellarium.BusinessLogic;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Repository;
using Xellarium.Shared;
using Xellarium.WebApi.V1;

namespace Xellarium.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await RunApp(args);
        }
        catch (HostAbortedException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
    
    private static void ConfigureBusinessLogicConfiguration(WebApplicationBuilder builder)
    {
        var businessLogicConfiguration = builder.Configuration.GetSection("BusinessLogic").Get<BusinessLogicConfiguration>();
        if (businessLogicConfiguration == null)
        {
            throw new InvalidOperationException("BusinessLogic configuration is missing");
        }
        builder.Services.AddSingleton(businessLogicConfiguration);
    }
    
    private static void ConfigureJwtAuthorizationConfiguration(WebApplicationBuilder builder)
    {
        var jwtAuthorizationConfiguration = builder.Configuration.GetSection("JwtAuthorization").Get<JwtAuthorizationConfiguration>();
        if (jwtAuthorizationConfiguration == null)
        {
            throw new InvalidOperationException("JwtAuthorization configuration is missing");
        }
        builder.Services.AddSingleton(jwtAuthorizationConfiguration);
    }
    
    public static async Task RunApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();
        
        ConfigureBusinessLogicConfiguration(builder);
        ConfigureJwtAuthorizationConfiguration(builder);
        
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Host.UseSerilog(logger);

        // Add services to the container.
        builder.ConfigureDatabase();
        
        var controllersBuilder = builder.Services.AddControllers();
        controllersBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(UserController).Assembly));
        controllersBuilder
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            .AddMvcOptions(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICollectionService, CollectionService>();
        builder.Services.AddScoped<IRuleService, RuleService>();
        builder.Services.AddScoped<INeighborhoodService, NeighborhoodService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        builder.Services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV";
            opt.SubstituteApiVersionInUrl = true;
        });
        
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add(HeaderNames.Accept);
            logging.RequestHeaders.Add(HeaderNames.ContentType);
            logging.RequestHeaders.Add(HeaderNames.ContentDisposition);
            logging.RequestHeaders.Add(HeaderNames.ContentEncoding);
            logging.RequestHeaders.Add(HeaderNames.ContentLength);
            logging.RequestHeaders.Add(HeaderNames.Cookie);
            
            logging.MediaTypeOptions.AddText("application/json");
            logging.MediaTypeOptions.AddText("multipart/form-data");
            
            logging.RequestBodyLogLimit = 1024;
            logging.ResponseBodyLogLimit = 1024;
        });
        
        builder.Services.AddCors(
            options => options.AddPolicy(
                "wasm",
                policy => policy.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials()));

        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

        builder.ConfigureMapping();
        builder.ConfigureJwtAuthentication();

        var app = builder.Build();

        app.UseCors("wasm");
        app.UseAuthentication();
        
        await DataAccessConfiguration.EnsureSeedData(app);

        app.UseDeveloperExceptionPage();
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/{documentName}/swagger.json";
        });
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint($"/api/{description.GroupName}/swagger.json", $"Xellarium API {description.GroupName}");
                opt.RoutePrefix = $"api/{description.GroupName.ToLower()}";
                logger.Information("Hosted swagger at {RoutePrefix}", opt.RoutePrefix);
            });
            
        }

        app.UseHttpsRedirection();
        
        app.UseAuthorization();

        app.MapControllers();

        app.UsePathBase("/legacy");

        await app.RunAsync();
    }
}