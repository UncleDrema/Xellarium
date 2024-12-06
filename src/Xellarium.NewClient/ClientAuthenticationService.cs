using Blazored.LocalStorage;
using Xellarium.NewClient.Authentication;

namespace Xellarium.NewClient;

public class ClientAuthenticationService(ILocalStorageService localStorage, TokenAuthenticationStateProvider tokenAuth)
{
    public async Task AuthenticateWithToken(string token)
    {
        await localStorage.SetItemAsync(TokenAuthenticationStateProvider.LocalStorageTokenKey, token);
        await localStorage.SetItemAsync(TokenAuthenticationStateProvider.LocalStorageIsAuthenticatedKey, "true");
        tokenAuth.MarkUserAsAuthenticated("user");
    }

    public async Task LogoutUser()
    {
        await tokenAuth.MarkUserAsLoggedOut();
        await localStorage.RemoveItemAsync(TokenAuthenticationStateProvider.LocalStorageIsAuthenticatedKey);
        await localStorage.RemoveItemAsync(TokenAuthenticationStateProvider.LocalStorageTokenKey);
    }
}