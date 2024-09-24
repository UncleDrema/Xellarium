using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface ICollectionService
{
    Task<IEnumerable<Collection>> GetCollections();
    Task<Collection?> GetCollection(int id);
    Task<Collection> AddCollection(Collection collection);
    Task UpdateCollection(Collection collection);
    Task DeleteCollection(int id);
    Task<IEnumerable<Collection>> GetRuleCollections(int ruleId);
    Task AddRule(int collectionId, Rule rule);
    Task RemoveRule(int collectionId, int ruleId);
    Task SetPrivacy(int id, bool isPrivate);
    Task<bool> CollectionExists(int id);
    Task<User?> GetOwner(int collectionId);
}