using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface ICollectionService
{
    Task<IEnumerable<Collection>> GetCollections();
    Task<IEnumerable<Collection>> GetPublicAndOwnedCollections(int userId);
    Task<IEnumerable<Collection>> GetPublicCollections();
    Task<Collection?> GetCollection(int id);
    Task AddCollection(Collection collection);
    Task UpdateCollection(Collection collection);
    Task DeleteCollection(int id);
    Task<IEnumerable<Rule>> GetCollectionRules(int collectionId);
    Task AddRule(int collectionId, Rule rule);
    Task RemoveRule(int collectionId, int ruleId);
    Task SetPrivacy(int id, bool isPrivate);
    Task<bool> CollectionExists(int id);
    Task<User?> GetOwner(int collectionId);
}