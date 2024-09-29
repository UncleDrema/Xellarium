using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Xellarium.Shared;
using Xellarium.Shared.DTO;

namespace Xellarium.WebApi.V2;

public static class HttpContextExtensions
{
    [ContractAnnotation(" => true, authUser: notnull; => false, authUser: null")]
    public static bool TryGetAuthenticatedUser(this HttpContext httpContext, out AuthenticatedUserDTO? authUser)
    {
        authUser = null;
        var token = httpContext.Request.Headers.Authorization.ToString();
        try
        {
            authUser = AuthorizationUtils.ParseJwt(token);
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}