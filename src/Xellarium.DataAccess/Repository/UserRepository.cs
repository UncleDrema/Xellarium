using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;
using Xellarium.Tracing;

namespace Xellarium.DataAccess.Repository;

public class UserRepository(XellariumContext context, ILogger logger)
    : GenericRepository<User>(context, logger), IUserRepository
{
    public async Task<User?> GetByName(string name)
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Users
            .Where(e => !e.IsDeleted)
            .Include(u => u.Collections.Where(e => !e.IsDeleted))
            .Include(u => u.Rules.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(u => u.Name == name);
    }
    
    public async Task<IEnumerable<User>> GetAllInclude()
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Users
            .Where(e => !e.IsDeleted)
            .Include(u => u.Collections.Where(e => !e.IsDeleted))
            .Include(u => u.Rules.Where(e => !e.IsDeleted))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<User>> GetAllByIdsInclude(IEnumerable<int> ids)
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Users
            .Where(e => !e.IsDeleted)
            .Include(u => u.Collections.Where(e => !e.IsDeleted))
            .Include(u => u.Rules.Where(e => !e.IsDeleted))
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }
    
    public async Task<User?> GetInclude(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return await _context.Users
            .Where(e => !e.IsDeleted)
            .Include(u => u.Collections.Where(e => !e.IsDeleted))
            .Include(u => u.Rules.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}