using System.Security.Claims;

namespace Xellarium.Shared.DTO;

public class AuthenticatedUserDTO
{
    public required int Id { get; init; } = -1;
    public required string Name { get; init; } = string.Empty;
    public required UserRole Role { get; init; } = UserRole.Guest;

    public bool CanAccessResourceOfUser(int userId)
    {
        return Role == UserRole.Admin || Id == userId;
    }

    public IEnumerable<Claim> ToClaims()
    {
        return new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.Role, Role.ToString())
        };
    }
}