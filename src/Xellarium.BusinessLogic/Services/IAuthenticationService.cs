using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public interface IAuthenticationService
{
    Task<User> RegisterUser(string name, string password, string? twoFactorSecret = null);
    Task<User?> AuthenticateUser(string name, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}