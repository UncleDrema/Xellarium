using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xellarium.DataAccess.Models;
using Xellarium.DataAccess.Repository;

namespace Xellarium.BusinessLogic.Test;

public class DatabaseFixture : IDisposable
{
    public XellariumContext Context { get; private set; }
    public RuleRepository RuleRepository { get; private set; }
    public UserRepository UserRepository { get; private set; }
    public CollectionRepository CollectionRepository { get; private set; }
    public NeighborhoodRepository NeighborhoodRepository { get; private set; }

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .Build();
        
        var connectionString = configuration.GetConnectionString("Postgres");
        
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseNpgsql(connectionString)  // Строка подключения из конфигурации тестов
            .Options;

        Context = new XellariumContext(options);
        Context.Database.Migrate(); // Применяем миграции
        
        RuleRepository = new RuleRepository(Context);
        UserRepository = new UserRepository(Context);
        CollectionRepository = new CollectionRepository(Context);
        NeighborhoodRepository = new NeighborhoodRepository(Context);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}