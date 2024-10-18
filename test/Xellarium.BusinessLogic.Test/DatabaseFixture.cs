using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;

namespace Xellarium.BusinessLogic.Test;

public class DatabaseFixture : IDisposable
{
    public XellariumContext Context { get; private set; }
    public UnitOfWork UnitOfWork { get; private set; }

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration.GetConnectionString("Postgres");
        
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseNpgsql(connectionString)  // Строка подключения из конфигурации тестов
            .Options;

        Context = new XellariumContext(options);
        Context.Database.Migrate(); // Применяем миграции

        UnitOfWork = new UnitOfWork(Context, new LoggerFactory().CreateLogger<UnitOfWork>());
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}