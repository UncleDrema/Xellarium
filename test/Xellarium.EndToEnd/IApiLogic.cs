using System.Threading.Tasks;
using Xellarium.Shared.DTO;

namespace Xellarium.EndToEnd;

public interface IApiLogic
{
    Task<bool> IsLoginSuccessful(UserLoginDTO loginDto);

    Task<bool> TryChangePassword(ChangePasswordDTO changePasswordDto);
}