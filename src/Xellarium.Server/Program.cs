using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Net.Http.Headers;
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
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static async Task RunApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var configuration = builder.Configuration.GetSection("BusinessLogic").Get<BusinessLogicConfiguration>();
        if (configuration == null)
        {
            throw new InvalidOperationException("BusinessLogic configuration is missing");
        }
        builder.Services.AddSingleton(configuration);
        
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
            });
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IRuleRepository, RuleRepository>();
        builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
        builder.Services.AddScoped<INeighborhoodRepository, NeighborhoodRepository>();

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
        builder.ConfigureAuthentication();

        var app = builder.Build();

        app.UseCors("wasm");
        app.UseAuthentication();
        
        await DataAccessConfiguration.EnsureSeedData(app);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    opt.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Xellarium API {description.GroupName}");
                }
            });
        }

        app.UseHttpsRedirection();
        
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}