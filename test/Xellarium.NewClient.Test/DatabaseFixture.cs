using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Services;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;
using Xellarium.Shared;
using Xunit.Abstractions;

namespace Xellarium.IntegrationTests;

public class DatabaseFixture : IDisposable
{
    public XellariumContext Context { get; private set; }
    public UnitOfWork UnitOfWork { get; private set; }

    private UserService _userService;
    private AuthenticationService _authService;
    private IConfiguration _configuration;
    private ITestOutputHelper _testOutputHelper;

    public DatabaseFixture(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = _configuration.GetConnectionString("Postgres");
        
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseNpgsql(connectionString)  // Строка подключения из конфигурации тестов
            .Options;

        Context = new XellariumContext(options);
        Context.Database.Migrate();

        UnitOfWork = new UnitOfWork(Context, new LoggerFactory().CreateLogger<UnitOfWork>());
        _userService = new UserService(UnitOfWork, new LoggerFactory().CreateLogger<UserService>()); 
        _authService = new AuthenticationService(_userService);
    }

    public async Task EnsureAdminExists()
    {
        var defaultUser = _configuration.GetSection("DefaultUser");
        
        if (!await _userService.UserExists(defaultUser["Username"]!))
            await _authService.RegisterUser(defaultUser["Username"]!, defaultUser["Password"]!, null);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}