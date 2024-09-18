using Microsoft.EntityFrameworkCore;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;

namespace Xellarium.DataAccess.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(XellariumContext context) : base(context)
    {
    }

    public async Task<User?> GetByName(string name)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
    }
}