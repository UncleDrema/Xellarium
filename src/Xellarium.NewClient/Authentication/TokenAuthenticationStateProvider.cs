using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Xellarium.Shared;

namespace Xellarium.NewClient.Authentication;

public class TokenAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    public const string LocalStorageTokenKey = "token";
    public const string LocalStorageIsAuthenticatedKey = "isAuthenticated";
    public const string AuthenticationScheme = "jwt";
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = await localStorage.GetItemAsync<string>(LocalStorageTokenKey);

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = AuthorizationUtils.ParseJwt(savedToken).ToClaims();
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme)));
    }

    public void MarkUserAsAuthenticated(string username)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, AuthenticationScheme));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.RemoveItemAsync(LocalStorageTokenKey);
        await localStorage.RemoveItemAsync(LocalStorageIsAuthenticatedKey);
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}