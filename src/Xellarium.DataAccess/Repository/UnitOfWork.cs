using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly XellariumContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    
    public IUserRepository Users { get; }
    
    public IRuleRepository Rules { get; }
    
    public ICollectionRepository Collections { get; }
    
    public INeighborhoodRepository Neighborhoods { get; }
    
    public UnitOfWork(XellariumContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
        Users = new UserRepository(context, logger);
        Rules = new RuleRepository(context, logger);
        Collections = new CollectionRepository(context, logger);
        Neighborhoods = new NeighborhoodRepository(context, logger);
    }
    
    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}