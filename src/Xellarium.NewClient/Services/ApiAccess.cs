using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;


public class ApiAccess(IHttpClientFactory httpClientFactory) : IApiAccess
{
    public const string ApiClientName = "API";

    private HttpClient Client => httpClientFactory.CreateClient(ApiClientName);

    private async Task<ResultCode> DeleteAsync(string uri)
    {
        var response = await Client.DeleteAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            return ResultCode.Ok;
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ResultCode.Unauthorized;
        }
        else
        {
            return ResultCode.Error;
        }
    }
    
    private async Task<(ResultCode, TRes?)> PostAsync<TBody, TRes>(string uri, TBody body)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(uri, content);
        if (response.IsSuccessStatusCode)
        {
            return (ResultCode.Ok, await response.Content.ReadFromJsonAsync<TRes>());
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, default);
        }
        else
        {
            return (ResultCode.Error, default);
        }
    }

    private async Task<(ResultCode, TRes?)> GetAsync<TRes>(string uri)
    {
        var response = await Client.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            return (ResultCode.Ok, await response.Content.ReadFromJsonAsync<TRes>());
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, default);
        }
        else
        {
            return (ResultCode.Error, default);
        }
    }
    
    private async Task<ResultCode> PostAsync<TBody>(string uri, TBody body)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client.PostAsync(uri, content);
        if (response.IsSuccessStatusCode)
        {
            return ResultCode.Ok;
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ResultCode.Unauthorized;
        }
        else
        {
            return ResultCode.Error;
        }
    }

    public async Task<GetUserResult> GetCurrentUser()
    {
        var (code, response) =  await GetAsync<UserDTO>("users/current");
        return new GetUserResult(code, response);
    }

    public async Task<GetUserResult> GetUser(int id)
    {
        var (code, response) =  await GetAsync<UserDTO>($"users/{id}");
        return new GetUserResult(code, response);
    }

    public async Task<GetNeighborhoodsResult> GetNeighborhoods()
    {
        var (code, response) =  await GetAsync<NeighborhoodDTO[]>("neighbourhoods");
        return new GetNeighborhoodsResult(code, response);
    }

    public async Task<GetNeighborhoodResult> GetNeighborhood(int id)
    {
        var (code, response) =  await GetAsync<NeighborhoodDTO>($"neighbourhoods/{id}");
        return new GetNeighborhoodResult(code, response);
    }

    public async Task<GetCollectionResult> GetCollection(int collectionId)
    {
        var (code, response) =  await GetAsync<CollectionDTO>($"collections/{collectionId}");
        return new GetCollectionResult(code, response);
    }

    public async Task<GetCollectionRulesResult> GetCollectionRules(int collectionId)
    {
        var (code, response) =  await GetAsync<RuleDTO[]>($"collections/{collectionId}/rules");
        return new GetCollectionRulesResult(code, response);
    }

    public async Task<GetPublicCollectionsResult> GetAvailableCollections()
    {
        var (code, response) =  await GetAsync<CollectionDTO[]>("collections");
        return new GetPublicCollectionsResult(code, response);
    }

    public async Task<AddCollectionResult> AddCollection(PostCollectionDTO collectionPostDto)
    {
        var (code, response) =  await PostAsync<PostCollectionDTO, CollectionDTO>("collections", collectionPostDto);
        return new AddCollectionResult(code, response);
    }

    public async Task<DeleteCollectionResult> DeleteCollection(int collectionId)
    {
        var code = await DeleteAsync($"collections/{collectionId}");
        return new DeleteCollectionResult(code);
    }

    public async Task<GetRuleResult> GetRule(int ruleId)
    {
        var (code, response) =  await GetAsync<RuleDTO>($"rules/{ruleId}");
        return new GetRuleResult(code, response);
    }

    public async Task<AddNewRuleToCollectionResult> AddNewRuleToCollection(int collectionId, PostRuleDTO rulePostDto)
    {
        var (code, response) =  await PostAsync<PostRuleDTO, RuleDTO>($"collections/{collectionId}/rules", rulePostDto);
        return new AddNewRuleToCollectionResult(code, response);
    }

    public async Task<AddRuleToCollectionResult> AddRuleToCollection(int collectionId, int ruleId)
    {
        var code = await PostAsync($"collections/{collectionId}", new RuleReferenceDTO
        {
            Id = ruleId
        });

        return new AddRuleToCollectionResult(code);
    }

    public async Task<DeleteRuleResult> DeleteRule(int ruleId)
    {
        var code = await DeleteAsync($"rules/{ruleId}");
        return new DeleteRuleResult(code);
    }

    public async Task<LoginResult> Login(UserLoginDTO userLoginDto)
    {
        var (code, response) =  await PostAsync<UserLoginDTO, AuthenticatedTokenDTO>("authentication/login", userLoginDto);
        return new LoginResult(code, response);
    }

    public async Task<RegisterResult> Register(UserRegisterDTO registerDto)
    {
        var (code, response) =  await PostAsync<UserRegisterDTO, RegisteredUserDTO>("authentication/register", registerDto);
        return new RegisterResult(code, response);
    }

    public async Task<Verify2FaResult> Verify2Fa(VerifyTwoFactorRequestDTO verifyTwoFactorRequestDto)
    {
        var (code, response) =  await PostAsync<VerifyTwoFactorRequestDTO, string>("authentication/verify-2fa", verifyTwoFactorRequestDto);
        return new Verify2FaResult(code, response);
    }
}