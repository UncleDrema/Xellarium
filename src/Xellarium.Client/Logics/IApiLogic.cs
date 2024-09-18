using Xellarium.Shared.DTO;

namespace Xellarium.Client.Logics;

public interface IApiLogic
{
    Task<ResultCode> Login(UserLoginDTO login);
    
    Task<ResultCode> Register(UserLoginDTO login);
    
    Task<ResultCode> Logout();

    Task<(ResultCode, UserDTO[])> GetAllUsers();
    
    Task<(ResultCode, NeighborhoodDTO[])> GetNeighborhoods();
    
    Task<(ResultCode, NeighborhoodDTO?)> GetNeighborhood(int id);
    
    Task<(ResultCode, UserDTO?)> GetProfile();
    
    Task<(ResultCode, UserDTO?)> GetUser(int id);
    
    Task DeleteUser(int id);
    
    Task WarnUser(int id);
    
    Task<(ResultCode, CollectionDTO?)> GetCollection(int collectionId);
    
    Task<RuleDTO> GetRule(int userId, int ruleId);
    
    Task<(ResultCode, IEnumerable<RuleDTO>?)> GetCollectionRules(int collectionId);
    
    Task<(ResultCode, IEnumerable<CollectionDTO>?)> GetPublicCollections();
    
    Task<(ResultCode, CollectionDTO?)> AddCollection(PostCollectionDTO collectionPostDto);
    
    Task<(ResultCode, RuleDTO?)> AddRule(int collectionId, PostRuleDTO ruleDto);
}