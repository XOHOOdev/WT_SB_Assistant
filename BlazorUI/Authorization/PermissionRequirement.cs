using Microsoft.AspNetCore.Authorization;

namespace Sparta.BlazorUI.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}