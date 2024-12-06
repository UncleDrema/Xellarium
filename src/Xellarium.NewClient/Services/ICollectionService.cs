using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct GetCollectionResult(ResultCode Result, CollectionDTO? Response);
public readonly record struct GetCollectionRulesResult(ResultCode Result, IEnumerable<RuleDTO>? Response);
public readonly record struct GetPublicCollectionsResult(ResultCode Result, IEnumerable<CollectionDTO>? Response);
public readonly record struct AddCollectionResult(ResultCode Result, CollectionDTO? Response);
public readonly record struct DeleteCollectionResult(ResultCode Result);

public interface ICollectionService
{
    Task<GetCollectionResult> GetCollection(int collectionId);

    Task<GetCollectionRulesResult> GetCollectionRules(int collectionId);

    Task<GetPublicCollectionsResult> GetAvailableCollections();

    Task<AddCollectionResult> AddCollection(PostCollectionDTO collectionPostDto);
    
    Task<DeleteCollectionResult> DeleteCollection(int collectionId);
}