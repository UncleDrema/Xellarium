using System;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reqnroll;
using Reqnroll.BoDi;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using Xellarium.BusinessLogic.Repository;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;

namespace Xellarium.EndToEnd;

public class HttpClientFixture
{
    private XellariumContext _context;
    private readonly IObjectContainer _objectContainer;

    public HttpClientFixture(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
    }

    [ScenarioDependencies]
    public static IServiceCollection CreateServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("Postgres");
        
        var services = new ServiceCollection();

        services.AddScoped<ILoggerFactory, LoggerFactory>();
        services.AddDbContext<XellariumContext>(options =>
            options
                .EnableSensitiveDataLogging()
                .UseNpgsql(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddHttpClient("API", options =>
        {
            options.BaseAddress = new Uri(configuration["ServerAddress"]!);
        });
        services.AddScoped<IApiLogic, ApiLogic>();

        return services;
    }

    /*
    [BeforeScenario]
    public void InitializeDatabaseConnection()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .Build();
        
        var connectionString = configuration.GetConnectionString("Postgres");
        
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseNpgsql(connectionString)  // Строка подключения из конфигурации тестов
            .Options;

        _context = new XellariumContext(options);
        _context.Database.Migrate(); // Применяем миграции

        var loggerFactory = new LoggerFactory();
        var unitOfWork = new UnitOfWork(_context, loggerFactory.CreateLogger<UnitOfWork>());
        var userService = new UserService(unitOfWork, loggerFactory.CreateLogger<UserService>());
        var authService = new AuthenticationService(userService);
        
        var client = new HttpClient()
        {
            BaseAddress = new Uri(configuration["ServerAddress"]!)
        };
        var logic = new ApiLogic(client);
        _objectContainer.RegisterInstanceAs<IApiLogic>(logic);
        _objectContainer.RegisterInstanceAs<IUnitOfWork>(unitOfWork);
        _objectContainer.RegisterInstanceAs<IAuthenticationService>(authService);
    }
    */
}