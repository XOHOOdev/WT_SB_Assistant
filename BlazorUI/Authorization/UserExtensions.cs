using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sparta.BlazorUI.Authorization;

public static class UserExtensions
{
    private static IAuthorizationHandler? _authorizationHandler;

    public static void Configure(IAuthorizationHandler? authorizationHandler)
    {
        _authorizationHandler = authorizationHandler;
    }

    public static bool Authorize(this ClaimsPrincipal user, string permission)
    {
        var context =
            new AuthorizationHandlerContext(
                new List<IAuthorizationRequirement> { new PermissionRequirement(permission) }, user, null);
        _authorizationHandler?.HandleAsync(context).Wait();

        return context.HasSucceeded;
    }
}