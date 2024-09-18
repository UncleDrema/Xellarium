using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly IRuleRepository _ruleRepository;
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public UserService(IUserRepository userRepository, ICollectionRepository collectionRepository,
        IRuleRepository ruleRepository, INeighborhoodRepository neighborhoodRepository)
    {
        _userRepository = userRepository;
        _collectionRepository = collectionRepository;
        _ruleRepository = ruleRepository;
        _neighborhoodRepository = neighborhoodRepository;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _userRepository.GetAll();
    }
    
    public async Task<IEnumerable<Neighborhood>> GetNeighborhoods()
    {
        return await _neighborhoodRepository.GetAll();
    }

    public async Task<User?> GetUser(int id)
    {
        return await _userRepository.Get(id);
    }
    
    public async Task<Neighborhood?> GetNeighborhood(int id)
    {
        return await _neighborhoodRepository.Get(id);
    }

    public async Task AddUser(User user)
    {
        if (await _userRepository.Exists(user.Id)) throw new ArgumentException("User already exists");
        await _userRepository.Add(user);
    }

    public async Task UpdateUser(User user)
    {
        if (!await _userRepository.Exists(user.Id)) throw new ArgumentException("User not found");
        await _userRepository.Update(user);
    }

    public async Task DeleteUser(int id)
    {
        var user = await _userRepository.Get(id, true);
        if (user == null || user.IsDeleted) throw new ArgumentException("User not found");
        await _userRepository.SoftDelete(id);
    }

    public async Task<IEnumerable<Collection>> GetUserCollections(int userId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Collections;
    }

    public async Task<IEnumerable<Rule>> GetUserRules(int userId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) throw new ArgumentException("User not found");
        return user.Rules;
    }

    public async Task WarnUser(int userId)
    {
        var user = await _userRepository.Get(userId);
        if (user != null)
        {
            user.AddWarning();
            await _userRepository.Update(user);
        }
        else
        {
            throw new ArgumentException("User not found");
        }
    }

    public Task<bool> UserExists(int id)
    {
        return _userRepository.Exists(id);
    }
    
    public async Task<bool> NameExists(string name)
    {
        return await _userRepository.GetByName(name) != null;
    }

    public async Task<Collection?> GetCollection(int collectionId)
    {
        return await _collectionRepository.Get(collectionId);
    }
    
    public async Task<Collection?> GetCollection(int userId, int collectionId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) return null;
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) return null;
        return collection.Owner.Id == user.Id ? collection : null;
    }
    
    public async Task<IEnumerable<Rule>> GetCollectionRules(int userId, int collectionId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) throw new ArgumentException("User not found");
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) throw new ArgumentException("Collection not found");
        return collection.Owner.Id == user.Id ? collection.Rules : Enumerable.Empty<Rule>();
    }
    
    public async Task<IEnumerable<Collection>> GetAccessibleCollections(int userId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) throw new ArgumentException("User not found");
        var collections = await _collectionRepository.GetAll();
        return collections.Where(c => user.Role == UserRole.Admin || !c.IsPrivate || c.Owner.Id == user.Id);
    }
    
    public async Task<Rule?> GetRule(int userId, int ruleId)
    {
        var user = await _userRepository.Get(userId);
        if (user == null) return null;
        var rule = await _ruleRepository.Get(ruleId);
        if (rule == null) return null;
        return rule.Owner.Id == user.Id ? rule : null;
    }
    
    public async Task<Rule?> GetRule(int ruleId)
    {
        return await _ruleRepository.Get(ruleId);
    }
    
    public async Task<Collection?> AddCollection(int id, Collection collection)
    {
        var user = await _userRepository.Get(id);
        if (user == null) return null;
        var addedCollection = await _collectionRepository.Add(collection, false);
        user.AddCollection(addedCollection);
        await _userRepository.Update(user);
        return addedCollection;
    }

    public async Task<Rule?> AddRule(int userId, Rule rule)
    {
        var user = await GetUser(userId);
        if (user == null) return null;
        var addedRule = await _ruleRepository.Add(rule, false);
        user.AddRule(addedRule);
        await _userRepository.Update(user);
        return addedRule;
    }

    public async Task AddToCollection(int collectionId, Rule rule)
    {
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) throw new ArgumentException("Collection not found");
        collection.AddRule(rule);
        await _collectionRepository.Update(collection);
    }

    public async Task<User> RegisterUser(string name, string password)
    {
        if (await NameExists(name)) throw new ArgumentException("User already exists");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is empty");
        
        var user = new User {Name = name, PasswordHash = HashPassword(password)};
        await AddUser(user);
        return user;
    }

    public async Task<User?> AuthenticateUser(string name, string password)
    {
        var user = await _userRepository.GetByName(name);
        if (user == null) return null;

        if (VerifyPassword(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}