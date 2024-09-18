using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Repository;

public interface IGenericRepository<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetAll(bool includeDeleted = false);
    Task<IEnumerable<T>> GetAllByIds(IEnumerable<int> ids, bool includeDeleted = false);
    Task<T?> Get(int id, bool includeDeleted = false);
    Task<T> Add(T entity, bool save = true);
    Task Update(T entity);
    Task SoftDelete(int id);
    Task HardDelete(int id);
    Task<bool> Exists(int id, bool includeDeleted = false);
}