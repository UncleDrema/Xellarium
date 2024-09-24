using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Services;

public class RuleService : IRuleService
{
    private readonly IRuleRepository _ruleRepository;
    private readonly ICollectionRepository _collectionRepository;

    public RuleService(IRuleRepository ruleRepository, ICollectionRepository collectionRepository)
    {
        _ruleRepository = ruleRepository;
        _collectionRepository = collectionRepository;
    }

    public async Task<IEnumerable<Rule>> GetRules()
    {
        return await _ruleRepository.GetAll();
    }

    public async Task<Rule?> GetRule(int id)
    {
        return await _ruleRepository.Get(id);
    }

    public async Task<Rule> AddRule(Rule rule)
    {
        if (await _ruleRepository.Exists(rule.Id)) throw new ArgumentException("Rule already exists");
        return await _ruleRepository.Add(rule);
    }

    public async Task UpdateRule(Rule rule)
    {
        if (!await _ruleRepository.Exists(rule.Id)) throw new ArgumentException("Rule not found");
        await _ruleRepository.Update(rule);
    }

    public async Task DeleteRule(int id)
    {
        var rule = await _ruleRepository.Get(id, true);
        if (rule == null || rule.IsDeleted) throw new ArgumentException("Rule not found");
        await _ruleRepository.SoftDelete(id);
    }

    public async Task<IEnumerable<Rule>> GetCollectionRules(int collectionId)
    {
        var collection = await _collectionRepository.Get(collectionId);
        if (collection == null) throw new ArgumentException("Collection not found");
        return collection.Rules;
    }

    public Task<bool> RuleExists(int id)
    {
        return _ruleRepository.Exists(id);
    }

    public async Task<User?> GetOwner(int ruleId)
    {
        var rule = await _ruleRepository.Get(ruleId);
        return rule?.Owner;
    }
}