using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByName(string name);
    
    public Task<IEnumerable<User>> GetAllInclude();
    
    public Task<IEnumerable<User>> GetAllByIdsInclude(IEnumerable<int> ids);
    
    public Task<User?> GetInclude(int id);
}