using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class RuleRepository(XellariumContext context, ILogger logger)
    : GenericRepository<Rule>(context, logger), IRuleRepository
{
    public async Task<IEnumerable<Rule>> GetAllInclude()
    {
        return await _context.Rules.Include(r => r.Owner)
            .Include(r => r.Collections)
            .Include(r => r.Neighborhood)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Rule>> GetAllByIdsInclude(IEnumerable<int> ids)
    {
        return await _context.Rules.Include(r => r.Owner)
            .Include(r => r.Collections)
            .Include(r => r.Neighborhood)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }

    public async Task<Rule?> GetInclude(int id)
    {
        return await _context.Rules.Include(r => r.Owner)
            .Include(r => r.Collections)
            .Include(r => r.Neighborhood)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}