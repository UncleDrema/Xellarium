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

    public async Task<ResultCode> Login(UserLoginDTO login)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/User/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            return ResultCode.Ok;
        }
        else if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return ResultCode.Error;
        }
        else
        {
            return ResultCode.Unauthorized;
        }
    }
    
    public async Task<ResultCode> Register(UserLoginDTO login)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(login);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/User/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            return ResultCode.Ok;
        }
        else if (response.StatusCode == HttpStatusCode.Conflict)
        {
            return ResultCode.Error;
        }
        else
        {
            return ResultCode.Unauthorized;
        }
    }
    
    public async Task<ResultCode> Logout()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsync("api/User/logout", null);
        if (response.IsSuccessStatusCode)
        {
            return ResultCode.Ok;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return ResultCode.Unauthorized;
        }
        else
        {
            return ResultCode.Error;
        }
    }

    public async Task<(ResultCode, UserDTO[])> GetAllUsers()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = (await client.GetFromJsonAsync<UserDTO[]>("api/User"))!;
        return (ResultCode.Ok, response);
    }
    
    public async Task<(ResultCode, UserDTO?)> GetUser(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/User/{id}");
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
        var response = (await client.GetFromJsonAsync<NeighborhoodDTO[]>("api/User/neighborhood"))!;
        return (ResultCode.Ok, response);
    }
    
    public async Task<(ResultCode, NeighborhoodDTO?)> GetNeighborhood(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/User/neighborhood/{id}");
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
        var neighborhood = await response.Content.ReadFromJsonAsync<NeighborhoodDTO>();
        return (ResultCode.Ok, neighborhood);
    }
    
    public async Task<(ResultCode, UserDTO?)> GetProfile()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("api/User/profile");
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
        await client.DeleteAsync($"api/User/{id}");
    }
    
    public async Task WarnUser(int id)
    {
        var client = _httpClientFactory.CreateClient("API");
        await client.PostAsync($"api/User/{id}/warn", null);
    }
    
    public async Task<(ResultCode, CollectionDTO?)> GetCollection(int collectionId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/User/collections/{collectionId}");
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
        var response = await client.GetAsync($"api/User/{userId}/rules/{ruleId}");
        var rule = await response.Content.ReadFromJsonAsync<RuleDTO>();
        return rule!;
    }
    
    public async Task<(ResultCode, IEnumerable<RuleDTO>?)> GetCollectionRules(int collectionId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/User/collections/{collectionId}/rules");
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (ResultCode.Unauthorized, Array.Empty<RuleDTO>());
        }
        else if (!response.IsSuccessStatusCode)
        {
            return (ResultCode.Error, Array.Empty<RuleDTO>());
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var rules = JsonSerializer.Deserialize<RuleDTO[]>(responseContent);
        return (ResultCode.Ok, rules);
    }

    public async Task<(ResultCode, IEnumerable<CollectionDTO>?)> GetPublicCollections()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("api/User/public_collections");
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
        var response = await client.PostAsync($"api/User/collections", content);

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
    
    public async Task<(ResultCode, RuleDTO?)> AddRule(int collectionId, PostRuleDTO ruleDto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(ruleDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"api/User/collections/{collectionId}/rule", content);
        
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
}