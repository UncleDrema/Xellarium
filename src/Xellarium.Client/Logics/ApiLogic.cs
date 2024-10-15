using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xellarium.Shared.DTO;

namespace Xellarium.Client.Logics;

public class ApiLogic : IApiLogic
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ApiLogic(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client() => _httpClientFactory.CreateClient("API");

    private async Task<ResultCode> PostAsync(string uri)
    {
        var response = await Client().PostAsync(uri, null);
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
            Console.WriteLine($"POST Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            return ResultCode.Error;
        }
    }
    
    private async Task<ResultCode> DeleteAsync(string uri)
    {
        var response = await Client().DeleteAsync(uri);
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
            Console.WriteLine($"DELETE Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            return ResultCode.Error;
        }
    }
    
    private async Task<ResultCode> PostAsync<TBody>(string uri, TBody body)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client().PostAsync(uri, content);
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
            Console.WriteLine($"POST Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            return ResultCode.Error;
        }
    }
    
    private async Task<(ResultCode, TRes?)> PostAsync<TBody, TRes>(string uri, TBody body)
    {
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client().PostAsync(uri, content);
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
            Console.WriteLine($"POST Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            return (ResultCode.Error, default);
        }
    }

    private async Task<(ResultCode, TRes?)> GetAsync<TRes>(string uri)
    {
        var response = await Client().GetAsync(uri);
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
            Console.WriteLine($"GET Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            return (ResultCode.Error, default);
        }
    }

    public async Task<(ResultCode, AuthenticatedTokenDTO?)> Login(UserLoginDTO login)
    {
        return await PostAsync<UserLoginDTO, AuthenticatedTokenDTO>("api/v2/authentication/login", login);
    }

    public async Task<(ResultCode, RegisteredUserDTO?)> Register(UserRegisterDTO login)
    {
        return await PostAsync<UserRegisterDTO, RegisteredUserDTO>("api/v2/authentication/register", login);
    }

    public async Task<ResultCode> Verify2Fa(VerifyTwoFactorRequestDTO verifyRequest)
    {
        return await PostAsync("api/v2/authentication/verify-2fa", verifyRequest);
    }

    public async Task<(ResultCode, UserDTO[]?)> GetAllUsers()
    {
        return await GetAsync<UserDTO[]>("api/v2/users");
    }

    public async Task<(ResultCode, NeighborhoodDTO[]?)> GetNeighborhoods()
    {
        return await GetAsync<NeighborhoodDTO[]>("api/v2/neighbourhoods");
    }

    public async Task<(ResultCode, NeighborhoodDTO?)> GetNeighborhood(int id)
    {
        return await GetAsync<NeighborhoodDTO>($"api/v2/neighbourhoods/{id}");
    }

    public async Task<(ResultCode, UserDTO?)> GetProfile()
    {
        return await GetAsync<UserDTO>("api/v2/users/current");
    }

    public async Task<(ResultCode, UserDTO?)> GetUser(int id)
    {
        return await GetAsync<UserDTO>($"api/v2/users/{id}");
    }

    public async Task<ResultCode> DeleteUser(int id)
    {
        return await DeleteAsync($"api/v2/users/{id}");
    }

    public async Task<ResultCode> WarnUser(int id)
    {
        return await PostAsync($"api/v2/users/{id}/warn");
    }

    public async Task<(ResultCode, CollectionDTO?)> GetCollection(int collectionId)
    {
        return await GetAsync<CollectionDTO>($"api/v2/collections/{collectionId}");
    }

    public async Task<(ResultCode, RuleDTO?)> GetRule(int ruleId)
    {
        return await GetAsync<RuleDTO>($"api/v2/rules/{ruleId}");
    }

    public async Task<(ResultCode, IEnumerable<RuleDTO>?)> GetCollectionRules(int collectionId)
    {
        return await GetAsync<RuleDTO[]>($"api/v2/collections/{collectionId}/rules");
    }

    public async Task<(ResultCode, IEnumerable<CollectionDTO>?)> GetPublicCollections()
    {
        return await GetAsync<CollectionDTO[]>($"api/v2/collections");
    }

    public async Task<(ResultCode, CollectionDTO?)> AddCollection(PostCollectionDTO collectionPostDto)
    {
        return await PostAsync<PostCollectionDTO, CollectionDTO>("api/v2/collections", collectionPostDto);
    }

    public async Task<(ResultCode, RuleDTO?)> AddRule(PostRuleDTO rulePostDto)
    {
        return await PostAsync<PostRuleDTO, RuleDTO>("api/v2/rules", rulePostDto);
    }

    public async Task<(ResultCode, RuleDTO?)> AddRuleToCollection(int collectionId, PostRuleDTO rulePostDto)
    {
        return await PostAsync<PostRuleDTO, RuleDTO>($"api/v2/collections/{collectionId}/rules", rulePostDto);
    }
}