namespace Xellarium.Shared.DTO;

public class AuthenticatedTokenDTO
{
    public string Token { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int ExpirationSeconds { get; set; } = -1;
}