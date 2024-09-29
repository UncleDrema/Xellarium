using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Repository;

public interface IRuleRepository : IGenericRepository<Rule>
{
    public Task<IEnumerable<Rule>> GetAllInclude();
    
    public Task<IEnumerable<Rule>> GetAllByIdsInclude(IEnumerable<int> ids);
    
    public Task<Rule?> GetInclude(int id);
}