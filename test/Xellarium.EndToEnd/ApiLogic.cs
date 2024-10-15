using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xellarium.Shared.DTO;

namespace Xellarium.EndToEnd;

public class ApiLogic : IApiLogic
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ApiLogic(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client() => _httpClientFactory.CreateClient("API");

    public bool IsLoginSuccessful(UserLoginDTO body)
    {
        var uri = "api/v2/authentication/login";
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = Client().PostAsync(uri, content).Result;
        return response.IsSuccessStatusCode;
    }
}