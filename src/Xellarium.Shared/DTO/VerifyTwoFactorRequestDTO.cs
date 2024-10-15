namespace Xellarium.Shared.DTO;

public class VerifyTwoFactorRequestDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}