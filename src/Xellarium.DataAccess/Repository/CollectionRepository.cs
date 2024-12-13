using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class CollectionRepository(XellariumContext context, ILogger logger)
    : GenericRepository<Collection>(context, logger), ICollectionRepository
{
    public async Task<IEnumerable<Collection>> GetPublicAndOwned(int userId)
    {
        return await _context.Collections
            .Where(e => !e.IsDeleted)
            .Include(col => col.Owner)
            .Include(col => col.Rules.Where(e => !e.IsDeleted))
            .Where(col => !col.IsPrivate || col.Owner.Id == userId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Collection>> GetPublic()
    {
        return await _context.Collections
            .Where(e => !e.IsDeleted)
            .Include(col => col.Owner)
            .Include(col => col.Rules.Where(e => !e.IsDeleted))
            .Where(col => !col.IsPrivate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Collection>> GetAllInclude()
    {
        return await _context.Collections
            .Where(e => !e.IsDeleted)
            .Include(col => col.Owner)
            .Include(col => col.Rules.Where(e => !e.IsDeleted))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Collection>> GetAllByIdsInclude(IEnumerable<int> ids)
    {
        return await _context.Collections.Include(col => col.Owner)
            .Include(col => col.Rules.Where(e => !e.IsDeleted))
            .Where(col => ids.Contains(col.Id))
            .ToListAsync();
    }
    
    public async Task<Collection?> GetInclude(int id)
    {
        return await _context.Collections.Include(col => col.Owner)
            .Include(col => col.Rules.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(col => col.Id == id);
    }
}