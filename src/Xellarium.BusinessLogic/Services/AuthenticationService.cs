using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Services;

public class AuthenticationService(IUserService userService) : IAuthenticationService
{
    public async Task<User> RegisterUser(string name, string password, string? twoFactorSecret)
    {
        if (await userService.UserExists(name)) throw new ArgumentException("User already exists");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is empty");
        
        var user = new User {Name = name, PasswordHash = HashPassword(password), TwoFactorSecret = twoFactorSecret};
        await userService.AddUser(user);
        return user;
    }

    public async Task<User?> AuthenticateUser(string name, string password)
    {
        var user = await userService.GetUserByName(name);
        if (user == null)
        {
            return null;
        }

        if (VerifyPassword(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}