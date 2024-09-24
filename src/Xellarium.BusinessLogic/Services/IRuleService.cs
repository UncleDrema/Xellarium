using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface IRuleService
{
    Task<IEnumerable<Rule>> GetRules();
    Task<Rule?> GetRule(int id);
    Task<Rule> AddRule(Rule rule);
    Task UpdateRule(Rule rule);
    Task DeleteRule(int id);
    Task<IEnumerable<Rule>> GetCollectionRules(int collectionId);
    Task<bool> RuleExists(int id);
    Task<User?> GetOwner(int ruleId);
}