using System.Diagnostics;
using Xellarium.BusinessLogic.Models;
using Xellarium.Tracing;

namespace Xellarium.BusinessLogic.Services;

public class AuthenticationService(IUserService userService) : IAuthenticationService
{
    public async Task<User> RegisterUser(string name, string password, string? twoFactorSecret)
    {
        using var activity = XellariumTracing.StartActivity();
        if (await userService.UserExists(name)) throw new ArgumentException("User already exists");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is empty");
        
        var user = new User {Name = name, PasswordHash = HashPassword(password), TwoFactorSecret = twoFactorSecret};
        await userService.AddUser(user);
        return user;
    }

    public async Task<User?> AuthenticateUser(string name, string password)
    {
        using var activity = XellariumTracing.StartActivity();
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
    
    public async Task ChangePassword(string name, string currentPassword, string newPassword)
    {
        using var activity = XellariumTracing.StartActivity();
        var user = await userService.GetUserByName(name);
        if (user == null) throw new ArgumentException("User not found");
        if (!VerifyPassword(currentPassword, user.PasswordHash)) throw new ArgumentException("Wrong password");
        
        user.PasswordHash = HashPassword(newPassword);
        await userService.UpdateUser(user);
    }

    public string HashPassword(string password)
    {
        using var activity = XellariumTracing.StartActivity();
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        using var activity = XellariumTracing.StartActivity();
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}