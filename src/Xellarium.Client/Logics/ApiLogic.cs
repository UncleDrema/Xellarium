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

    public async Task<(ResultCode, AuthenticatedTokenDTO?)> Login(UserLoginDTO login)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/v2/authentication/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticatedTokenDTO>();
            return (ResultCode.Ok, authResponse);
        }
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return (ResultCode.Error, null);
        }
        else
        {
            return (ResultCode.Unauthorized, null);
        }
    }
    
    public async Task<(ResultCode, AuthenticatedTokenDTO?)> Register(UserLoginDTO login)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/v2/authentication/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticatedTokenDTO>();
            return (ResultCode.Ok, authResponse);
        }
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return (ResultCode.Error, null);
        }
        else
        {
            return (ResultCode.Unauthorized, null);
        }
    }

    public async Task<(ResultCode, UserDTO[])> GetAllUsers()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = (await client.GetFromJsonAsync<UserDTO[]>("api/v2/users"))!;
        return (ResultCode.Ok, response);
    }
    
    public async Task<(ResultCode, UserDTO?)> GetUser(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/users/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return (ResultCode.Error, null);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        var user = await response.Content.ReadFromJsonAsync<UserDTO>();
        return (ResultCode.Ok, user);
    }
    
    public async Task<(ResultCode, NeighborhoodDTO[])> GetNeighborhoods()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = (await client.GetFromJsonAsync<NeighborhoodDTO[]>("api/v2/neighbourhoods"))!;
        return (ResultCode.Ok, response);
    }
    
    public async Task<(ResultCode, NeighborhoodDTO?)> GetNeighborhood(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/neighbourhoods/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return (ResultCode.Error, null);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        var neighbourhood = await response.Content.ReadFromJsonAsync<NeighborhoodDTO>();
        return (ResultCode.Ok, neighbourhood);
    }
    
    public async Task<(ResultCode, UserDTO?)> GetProfile()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("api/v2/profile");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        var user = await response.Content.ReadFromJsonAsync<UserDTO>();
        return (ResultCode.Ok, user);
    }
    
    public async Task DeleteUser(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        await client.DeleteAsync($"api/v2/users/{id}");
    }
    
    public async Task WarnUser(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        await client.PostAsync($"api/v2/users/{id}/warn", null);
    }
    
    public async Task<(ResultCode, CollectionDTO?)> GetCollection(int collectionId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/collections/{collectionId}");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        
        var collection = await response.Content.ReadFromJsonAsync<CollectionDTO>();
        return (ResultCode.Ok, collection);
    }
    
    public async Task<RuleDTO> GetRule(int userId, int ruleId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/users/{userId}/rules/{ruleId}");
        var rule = await response.Content.ReadFromJsonAsync<RuleDTO>();
        return rule!;
    }

    public async Task<RuleDTO> GetRule(int ruleId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/rules/{ruleId}");
        var rule = await response.Content.ReadFromJsonAsync<RuleDTO>();
        return rule!;
    }
    
    public async Task<(ResultCode, IEnumerable<RuleReferenceDTO>?)> GetCollectionRules(int collectionId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/v2/collections/{collectionId}/contained_rules");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, Array.Empty<RuleReferenceDTO>());
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, Array.Empty<RuleReferenceDTO>());
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var rules = JsonSerializer.Deserialize<RuleReferenceDTO[]>(responseContent);
        return (ResultCode.Ok, rules);
    }

    public async Task<(ResultCode, IEnumerable<CollectionDTO>?)> GetPublicCollections()
    {
        var client = _httpClientFactory.CreateClient("API");
        
        var response = await client.GetAsync("api/v2/users/1/accessibleCollections");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var collections = JsonSerializer.Deserialize<CollectionDTO[]>(responseContent);
        return (ResultCode.Ok, collections);
    }
    
    public async Task<(ResultCode, CollectionDTO?)> AddCollection(PostCollectionDTO collectionPostDto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(collectionPostDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"api/v2/users/collections", content);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<CollectionDTO>(responseContent);
            
            return (ResultCode.Ok, responseDto);
        }
    }
    
    public async Task<(ResultCode, RuleDTO?)> AddRule(int userId, PostRuleDTO ruleDto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(ruleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"api/v2/users/{userId}/rules", content);
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, null);
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, null);
        }
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<RuleDTO>(responseContent);
            return (ResultCode.Ok, responseDto);
        }
    }

    public async Task AddRuleToCollection(int collectionid, int ruleId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(new RuleReferenceDTO
        {
            Id = ruleId
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"api/v2/collections/contained_rules", content);
    }
}