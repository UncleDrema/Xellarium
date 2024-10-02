using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Services;

public class UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<User>> GetUsers()
    {
        return await unitOfWork.Users.GetAllInclude();
    }
    
    public async Task<IEnumerable<Neighborhood>> GetNeighborhoods()
    {
        return await unitOfWork.Neighborhoods.GetAll();
    }

    public async Task<User?> GetUser(int id)
    {
        return await unitOfWork.Users.GetInclude(id);
    }
    
    public async Task<Neighborhood?> GetNeighborhood(int id)
    {
        return await unitOfWork.Neighborhoods.Get(id);
    }

    public async Task AddUser(User user)
    {
        if (await unitOfWork.Users.Exists(user.Id)) throw new ArgumentException("User already exists");
        await unitOfWork.Users.Add(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateUser(User user)
    {
        if (!await unitOfWork.Users.Exists(user.Id)) throw new ArgumentException("User not found");
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await unitOfWork.Users.Get(id);
        if (user == null) throw new ArgumentException("User not found");
        await unitOfWork.Users.SoftDelete(id);
        await unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<Collection>> GetUserCollections(int userId)
    {
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Collections;
    }

    public async Task<IEnumerable<Rule>> GetUserRules(int userId)
    {
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Rules;
    }

    public async Task WarnUser(int userId)
    {
        var user = await unitOfWork.Users.Get(userId);
        if (user != null)
        {
            user.AddWarning();
            await unitOfWork.Users.Update(user);
            await unitOfWork.CompleteAsync();
        }
        else
        {
            throw new ArgumentException("User not found");
        }
    }

    public Task<bool> UserExists(int id)
    {
        return unitOfWork.Users.Exists(id);
    }
    
    public async Task<User?> GetUserByName(string name)
    {
        return await unitOfWork.Users.GetByName(name);
    }
    
    public async Task<bool> UserExists(string name)
    {
        return (await GetUserByName(name)) != null;
    }

    public async Task<Collection?> GetCollection(int collectionId)
    {
        return await unitOfWork.Collections.Get(collectionId);
    }
    
    public async Task<Collection?> GetCollection(int userId, int collectionId)
    {
        var user = await unitOfWork.Users.Get(userId);
        if (user == null) return null;
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) return null;
        return collection.Owner.Id == user.Id ? collection : null;
    }
    
    public async Task<Rule?> GetRule(int userId, int ruleId)
    {
        var user = await unitOfWork.Users.Get(userId);
        if (user == null) return null;
        var rule = await unitOfWork.Rules.GetInclude(ruleId);
        if (rule == null) return null;
        return rule.Owner.Id == user.Id ? rule : null;
    }
    
    public async Task AddCollection(int id, Collection collection)
    {
        var user = await unitOfWork.Users.GetInclude(id);
        if (user == null) throw new ArgumentException("User not found");
        user.AddCollection(collection);
        await unitOfWork.Collections.Add(collection);
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddRule(int userId, Rule rule)
    {
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        user.AddRule(rule);
        await unitOfWork.Rules.Add(rule);
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddNewRuleToCollection(int userId, int collectionId, Rule rule)
    {
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) throw new ArgumentException("Collection not found");
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        if (collection.Owner.Id != user.Id) throw new ArgumentException("User is not the owner of the collection");
        if (collection.Rules.Any(r => r.Id == rule.Id)) throw new ArgumentException("Rule already exists in the collection");
        user.AddRule(rule);
        collection.AddRule(rule);
        await unitOfWork.Rules.Add(rule);
        await unitOfWork.Users.Update(user);
        await unitOfWork.Collections.Update(collection);
        await unitOfWork.CompleteAsync();
    }
}