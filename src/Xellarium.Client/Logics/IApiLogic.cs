using Xellarium.Shared.DTO;

namespace Xellarium.Client.Logics;

public interface IApiLogic
{
    Task<(ResultCode, AuthenticatedTokenDTO?)> Login(UserLoginDTO login);
    
    Task<(ResultCode, AuthenticatedTokenDTO?)> Register(UserLoginDTO login);

    Task<(ResultCode, UserDTO[]?)> GetAllUsers();
    
    Task<(ResultCode, NeighborhoodDTO[]?)> GetNeighborhoods();
    
    Task<(ResultCode, NeighborhoodDTO?)> GetNeighborhood(int id);
    
    Task<(ResultCode, UserDTO?)> GetProfile();
    
    Task<(ResultCode, UserDTO?)> GetUser(int id);
    
    Task<ResultCode> DeleteUser(int id);
    
    Task<ResultCode> WarnUser(int id);
    
    Task<(ResultCode, CollectionDTO?)> GetCollection(int collectionId);

    Task<(ResultCode, RuleDTO?)> GetRule(int ruleId);
    
    Task<(ResultCode, IEnumerable<RuleDTO>?)> GetCollectionRules(int collectionId);
    
    Task<(ResultCode, IEnumerable<CollectionDTO>?)> GetPublicCollections();
    
    Task<(ResultCode, CollectionDTO?)> AddCollection(PostCollectionDTO collectionPostDto);

    Task<(ResultCode, RuleDTO?)> AddRule(PostRuleDTO rulePostDto);

    Task<(ResultCode, RuleDTO?)> AddRuleToCollection(int collectionId, PostRuleDTO rulePostDto);
}