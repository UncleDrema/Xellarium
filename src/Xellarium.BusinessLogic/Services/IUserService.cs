using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsers();
    Task<IEnumerable<Neighborhood>> GetNeighborhoods();
    Task<User?> GetUser(int id);
    Task<Neighborhood?> GetNeighborhood(int id);
    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(int id);
    Task<IEnumerable<Collection>> GetUserCollections(int userId);
    Task<IEnumerable<Rule>> GetUserRules(int userId);
    Task WarnUser(int userId);
    Task<bool> UserExists(int id);
    Task<bool> NameExists(string name);
    Task<Collection?> GetCollection(int collectionId);
    Task<Collection?> GetCollection(int userId, int collectionId);
    Task<IEnumerable<Rule>> GetCollectionRules(int userId, int collectionId);
    Task<IEnumerable<Collection>> GetAccessibleCollections(int userId);
    Task<Rule?> GetRule(int userId, int ruleId);
    Task<Rule?> GetRule(int ruleId);
    Task<Collection?> AddCollection(int id, Collection collection);
    Task<Rule?> AddRule(int userId, Rule rule);
    Task AddToCollection(int collectionId, Rule rule);
    Task<User> RegisterUser(string name, string password);
    Task<User?> AuthenticateUser(string name, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}