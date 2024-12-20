using System.Security.Claims;
using System.Text.Json;
using Xellarium.Shared.DTO;
using Xellarium.Tracing;

namespace Xellarium.Shared;

public static class AuthorizationUtils
{
    public static AuthenticatedUserDTO ParseJwt(string jwt)
    {
        using var activity = XellariumTracing.StartActivity();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var parsedClaims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)!;

        return new AuthenticatedUserDTO
        {
            Id = int.Parse(parsedClaims[ClaimTypes.NameIdentifier].ToString()!),
            Name = parsedClaims[ClaimTypes.Name].ToString()!,
            Role = Enum.Parse<UserRole>(parsedClaims[ClaimTypes.Role].ToString()!)
        };
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        using var activity = XellariumTracing.StartActivity();
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}