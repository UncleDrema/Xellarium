using Xellarium.Shared.DTO;

namespace Xellarium.EndToEnd;

public interface IApiLogic
{
    bool IsLoginSuccessful(UserLoginDTO loginDto);
}