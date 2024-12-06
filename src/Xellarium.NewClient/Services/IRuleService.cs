using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct GetRuleResult(ResultCode Result, RuleDTO? Response);
public readonly record struct AddNewRuleToCollectionResult(ResultCode Result, RuleDTO? Response);

public readonly record struct AddRuleToCollectionResult(ResultCode Result);
public readonly record struct DeleteRuleResult(ResultCode Result);

public interface IRuleService
{
    Task<GetRuleResult> GetRule(int ruleId);

    Task<AddNewRuleToCollectionResult> AddNewRuleToCollection(int collectionId, PostRuleDTO rulePostDto);

    Task<AddRuleToCollectionResult> AddRuleToCollection(int collectionId, int ruleId);

    Task<DeleteRuleResult> DeleteRule(int ruleId);
}