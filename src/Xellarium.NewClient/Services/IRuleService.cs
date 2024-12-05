using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct GetRuleResult(ResultCode Result, RuleDTO? Response);
public readonly record struct AddRuleToCollectionResult(ResultCode Result, RuleDTO? Response);

public interface IRuleService
{
    Task<GetRuleResult> GetRule(int ruleId);

    Task<AddRuleToCollectionResult> AddRuleToCollection(int collectionId, PostRuleDTO rulePostDto);
}