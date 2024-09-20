using Microsoft.EntityFrameworkCore;
using Xellarium.BusinessLogic.Models;
using Xellarium.DataAccess.Models;

namespace Xellarium.IntegrationTests;

public class TestDatabaseBuilder
{
    private readonly XellariumContext _context;

    public TestDatabaseBuilder()
    {
        var options = new DbContextOptionsBuilder<XellariumContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new XellariumContext(options);
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        _context.Database.EnsureCreatedAsync();
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