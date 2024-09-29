using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;

namespace Xellarium.BusinessLogic.Services;

public class RuleService(IUnitOfWork unitOfWork)
    : IRuleService
{
    public async Task<IEnumerable<Rule>> GetRules()
    {
        return await unitOfWork.Rules.GetAll();
    }

    public async Task<Rule?> GetRule(int id)
    {
        return await unitOfWork.Rules.Get(id);
    }

    public async Task AddRule(Rule rule)
    {
        if (await unitOfWork.Rules.Exists(rule.Id)) throw new ArgumentException("Rule already exists");
        await unitOfWork.Rules.Add(rule);
        await unitOfWork.CompleteAsync();
    }

    public async Task UpdateRule(Rule rule)
    {
        if (!await unitOfWork.Rules.Exists(rule.Id)) throw new ArgumentException("Rule not found");
        await unitOfWork.Rules.Update(rule);
        await unitOfWork.CompleteAsync();
    }

    public async Task DeleteRule(int id)
    {
        var rule = await unitOfWork.Rules.Get(id);
        if (rule == null) throw new ArgumentException("Rule not found");
        await unitOfWork.Rules.SoftDelete(id);
        await unitOfWork.CompleteAsync();
    }
    
    public async Task<IEnumerable<Collection>> GetRuleCollections(int ruleId)
    {
        var rule = await unitOfWork.Rules.Get(ruleId);
        if (rule == null) throw new ArgumentException("Rule not found");
        return rule.Collections;
    }

    public Task<bool> RuleExists(int id)
    {
        return unitOfWork.Rules.Exists(id);
    }

    public async Task<User?> GetOwner(int ruleId)
    {
        var rule = await unitOfWork.Rules.Get(ruleId);
        return rule?.Owner;
    }
}