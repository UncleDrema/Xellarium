using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByName(string name);
}