using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct LoginResult(ResultCode Result, AuthenticatedTokenDTO? Response);
public readonly record struct RegisterResult(ResultCode Result, RegisteredUserDTO? Response);
public readonly record struct Verify2FaResult(ResultCode Result, string? Response);

public interface IAuthenticationService
{
    Task<LoginResult> Login(UserLoginDTO userLoginDto);

    Task<RegisterResult> Register(UserRegisterDTO registerDto);

    Task<Verify2FaResult> Verify2Fa(VerifyTwoFactorRequestDTO verifyTwoFactorRequestDto);
}