using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xellarium.BusinessLogic.Models;
using Xellarium.DataAccess.Models;

namespace Xellarium.IntegrationTests;

public class TestDatabaseBuilder
{
    private readonly XellariumContext _context;

    public TestDatabaseBuilder()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("Postgres");
        
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        _context = new XellariumContext(options);
        _context.Database.Migrate();
    }
    
    public TestDatabaseBuilder WithUsers(IEnumerable<User> users)
    {
        _context.Users.AddRange(users);
        return this;
    }
    
    public TestDatabaseBuilder WithCollections(IEnumerable<Collection> collections)
    {
        _context.Collections.AddRange(collections);
        return this;
    }
    
    public TestDatabaseBuilder WithNeighborhoods(IEnumerable<Neighborhood> neighborhoods)
    {
        _context.Neighborhoods.AddRange(neighborhoods);
        return this;
    }
    
    public TestDatabaseBuilder WithRules(IEnumerable<Rule> rules)
    {
        _context.Rules.AddRange(rules);
        return this;
    }
    
    public XellariumContext Build()
    {
        _context.SaveChanges();
        return _context;
    }
}