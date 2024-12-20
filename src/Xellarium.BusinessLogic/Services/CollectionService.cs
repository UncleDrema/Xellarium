using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Services;

public class CollectionService(IUnitOfWork unitOfWork)
    : ICollectionService
{
    public async Task<IEnumerable<Collection>> GetCollections()
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Collections.GetAllInclude();
    }
    
    public async Task<IEnumerable<Collection>> GetPublicAndOwnedCollections(int userId)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Collections.GetPublicAndOwned(userId);
    }

    public async Task<IEnumerable<Collection>> GetPublicCollections()
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Collections.GetPublic();
    }

    public async Task<Collection?> GetCollection(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Collections.GetInclude(id);
    }

    public async Task AddCollection(Collection collection)
    {
        using var activity = XellariumTracing.StartActivity();
        if (await unitOfWork.Collections.Exists(collection.Id)) throw new ArgumentException("Collection already exists");
        await unitOfWork.Collections.Add(collection);
        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateCollection(Collection collection)
    {
        using var activity = XellariumTracing.StartActivity();
        if (!await unitOfWork.Collections.Exists(collection.Id)) throw new ArgumentException("Collection not found");
        await unitOfWork.Collections.Update(collection);
        await unitOfWork.CompleteAsync();
        
    }

    public async Task DeleteCollection(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.Get(id);
        if (collection == null) throw new ArgumentException("Collection not found");
        await unitOfWork.Collections.SoftDelete(id);
        await unitOfWork.CompleteAsync();
    }
    
    public async Task<IEnumerable<Rule>> GetCollectionRules(int collectionId)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) throw new ArgumentException("Collection not found");
        return collection.Rules;
    }

    public async Task AddRule(int collectionId, Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) return;
        collection.AddRule(rule);
        await unitOfWork.Collections.Update(collection);
        await unitOfWork.CompleteAsync();
    }
    
    public async Task RemoveRule(int collectionId, int ruleId)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) return;
        
        collection.RemoveRule(collection.Rules.First(r => r.Id == ruleId));
        await unitOfWork.Collections.Update(collection);
        await unitOfWork.CompleteAsync();
    }
    
    public async Task SetPrivacy(int id, bool isPrivate)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.Get(id);
        if (collection == null) throw new ArgumentException("Collection not found");
        collection.IsPrivate = isPrivate;
        await unitOfWork.Collections.Update(collection);
        await unitOfWork.CompleteAsync();
    }

    public Task<bool> CollectionExists(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return unitOfWork.Collections.Exists(id);
    }

    public async Task<User?> GetOwner(int collectionId)
    {
        using var activity = XellariumTracing.StartActivity();
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        return collection?.Owner;
    }
}