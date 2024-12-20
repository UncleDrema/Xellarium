using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;
using Xellarium.Tracing;

namespace Xellarium.DataAccess.Repository;

public class RuleRepository(XellariumContext context, ILogger logger)
    : GenericRepository<Rule>(context, logger), IRuleRepository
{
    public async Task<IEnumerable<Rule>> GetAllInclude()
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Rules
            .Where(e => !e.IsDeleted)
            .Include(r => r.Owner)
            .Include(r => r.Collections.Where(e => !e.IsDeleted))
            .Include(r => r.Neighborhood)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Rule>> GetAllByIdsInclude(IEnumerable<int> ids)
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Rules
            .Where(e => !e.IsDeleted)
            .Include(r => r.Owner)
            .Include(r => r.Collections.Where(e => !e.IsDeleted))
            .Include(r => r.Neighborhood)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Rule?> GetInclude(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Rules
            .Where(e => !e.IsDeleted)
            .Include(r => r.Owner)
            .Include(r => r.Collections.Where(e => !e.IsDeleted))
            .Include(r => r.Neighborhood)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}