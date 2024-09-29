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
    Task<User?> GetUserByName(string name);
    Task<bool> UserExists(string name);
    Task<Collection?> GetCollection(int collectionId);
    Task<Collection?> GetCollection(int userId, int collectionId);
    Task<Rule?> GetRule(int userId, int ruleId);
    Task AddCollection(int id, Collection collection);
    Task AddRule(int userId, Rule rule);
    Task AddNewRuleToCollection(int userId, int collectionId, Rule rule);
}