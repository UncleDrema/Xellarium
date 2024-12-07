using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

    public async Task<bool> IsLoginSuccessful(UserLoginDTO loginDto)
    {
        var uri = "api/v2/authentication/login";
        var json = JsonSerializer.Serialize(loginDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client().PostAsync(uri, content);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> TryChangePassword(ChangePasswordDTO changePasswordDto)
    {
        var uri = "api/v2/authentication/change-password";
        var json = JsonSerializer.Serialize(changePasswordDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await Client().PostAsync(uri, content);
        return response.IsSuccessStatusCode;
    }
}