namespace Xellarium.Shared.DTO;

public class RegisteredUserDTO
{
    public string Token { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int ExpirationSeconds { get; set; } = -1;
    public string TwoFactorQrCodeUri { get; set; } = string.Empty;
}