using Xellarium.Shared.DTO;

namespace Xellarium.NewClient.Services;

public readonly record struct GetUserResult(ResultCode Result, UserDTO? Response);

public interface IUserService
{
    Task<GetUserResult> GetCurrentUser();

    Task<GetUserResult> GetUser(int id);
}