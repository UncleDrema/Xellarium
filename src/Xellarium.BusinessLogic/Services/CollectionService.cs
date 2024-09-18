using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Services;

public class CollectionService : ICollectionService
{
    private readonly ICollectionRepository _collectionRepository;
    private readonly IRuleRepository _ruleRepository;

    public CollectionService(ICollectionRepository collectionRepository, IRuleRepository ruleRepository)
    {
        _collectionRepository = collectionRepository;
        _ruleRepository = ruleRepository;
    }

    public async Task<IEnumerable<Collection>> GetCollections()
    {
        return await _collectionRepository.GetAll();
    }

    public async Task<Collection?> GetCollection(int id)
    {
        return await _collectionRepository.Get(id);
    }

    public async Task AddCollection(Collection collection)
    {
        if (await _collectionRepository.Exists(collection.Id)) throw new ArgumentException("Collection already exists");
        await _collectionRepository.Add(collection);
    }

    public async Task UpdateCollection(Collection collection)
    {
        if (!await _collectionRepository.Exists(collection.Id)) throw new ArgumentException("Collection not found");
        await _collectionRepository.Update(collection);
    }

    public async Task DeleteCollection(int id)
    {
        var collection = await _collectionRepository.Get(id, true);
        if (collection == null || collection.IsDeleted) throw new ArgumentException("Collection not found");
        await _collectionRepository.SoftDelete(id);
    }

    public async Task<IEnumerable<Collection>> GetRuleCollections(int ruleId)
    {
        var rule = await _ruleRepository.Get(ruleId);
        if (rule == null) throw new ArgumentException("Rule not found");
        return rule.Collections;
    }

    public async Task AddRule(int collectionId, Rule rule)
    {
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) return;
        await _ruleRepository.Add(rule);
        collection.AddRule(rule);
        await _collectionRepository.Update(collection);
    }
    
    public async Task RemoveRule(int collectionId, Rule rule)
    {
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) return;
        collection.RemoveRule(rule);
        await _collectionRepository.Update(collection);
    }
    
    public async Task SetPrivacy(int id, bool isPrivate)
    {
        var collection = await _collectionRepository.Get(id);
        if (collection == null) throw new ArgumentException("Collection not found");
        collection.IsPrivate = isPrivate;
        await _collectionRepository.Update(collection);
    }

    public Task<bool> CollectionExists(int id)
    {
        return _collectionRepository.Exists(id);
    }

    public async Task<User?> GetOwner(int collectionId)
    {
        var collection = await _collectionRepository.Get(collectionId);
        return collection?.Owner;
    }
}