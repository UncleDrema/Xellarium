using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Repository;

public interface ICollectionRepository : IGenericRepository<Collection>
{
    public Task<IEnumerable<Collection>> GetPublicAndOwned(int userId);
    
    public Task<IEnumerable<Collection>> GetPublic();
    
    public Task<IEnumerable<Collection>> GetAllInclude();
    
    public Task<IEnumerable<Collection>> GetAllByIdsInclude(IEnumerable<int> ids);
    
    public Task<Collection?> GetInclude(int id);
}