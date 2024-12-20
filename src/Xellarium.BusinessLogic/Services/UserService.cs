using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.Shared;
using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Services;

public class UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger) : IUserService
{
    public async Task<IEnumerable<User>> GetUsers()
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Users.GetAllInclude();
    }
    
    public async Task<IEnumerable<Neighborhood>> GetNeighborhoods()
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Neighborhoods.GetAll();
    }

    public async Task<User?> GetUser(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Users.GetInclude(id);
    }
    
    public async Task<Neighborhood?> GetNeighborhood(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Neighborhoods.Get(id);
    }

    public async Task AddUser(User user)
    {
        using var activity = XellariumTracing.StartActivity();
        if (await unitOfWork.Users.Exists(user.Id)) throw new ArgumentException("User already exists");
        await unitOfWork.Users.Add(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateUser(User user)
    {
        using var activity = XellariumTracing.StartActivity();
        if (!await unitOfWork.Users.Exists(user.Id)) throw new ArgumentException("User not found");
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteUser(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.Get(id);
        if (user == null) throw new ArgumentException("User not found");
        await unitOfWork.Users.SoftDelete(id);
        await unitOfWork.CompleteAsync();
    }

    public async Task<IEnumerable<Collection>> GetUserCollections(int userId)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Collections;
    }

    public async Task<IEnumerable<Rule>> GetUserRules(int userId)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Rules;
    }

    public async Task WarnUser(int userId)
    {
        using var activity = XellariumTracing.StartActivity();
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
        using var activity = XellariumTracing.StartActivity();
        return unitOfWork.Users.Exists(id);
    }
    
    public async Task<User?> GetUserByName(string name)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Users.GetByName(name);
    }
    
    public async Task<bool> UserExists(string name)
    {
        using var activity = XellariumTracing.StartActivity();
        return (await GetUserByName(name)) != null;
    }

    public async Task<Collection?> GetCollection(int collectionId)
    {
        using var activity = XellariumTracing.StartActivity();
        return await unitOfWork.Collections.Get(collectionId);
    }
    
    public async Task<Collection?> GetCollection(int userId, int collectionId)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.Get(userId);
        if (user == null) return null;
        var collection = await unitOfWork.Collections.GetInclude(collectionId);
        if (collection == null) return null;
        return collection.Owner.Id == user.Id ? collection : null;
    }
    
    public async Task<Rule?> GetRule(int userId, int ruleId)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.Get(userId);
        if (user == null) return null;
        var rule = await unitOfWork.Rules.GetInclude(ruleId);
        if (rule == null) return null;
        return rule.Owner.Id == user.Id ? rule : null;
    }
    
    public async Task AddCollection(int id, Collection collection)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.GetInclude(id);
        if (user == null) throw new ArgumentException("User not found");
        user.AddCollection(collection);
        await unitOfWork.Collections.Add(collection);
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddRule(int userId, Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await unitOfWork.Users.GetInclude(userId);
        if (user == null) throw new ArgumentException("User not found");
        user.AddRule(rule);
        await unitOfWork.Rules.Add(rule);
        await unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();
    }

    public async Task AddNewRuleToCollection(int userId, int collectionId, Rule rule)
    {
        using var activity = XellariumTracing.StartActivity();
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