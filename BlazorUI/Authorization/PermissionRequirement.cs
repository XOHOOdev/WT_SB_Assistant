using Microsoft.AspNetCore.Authorization;

namespace WtSbAssistant.BlazorUI.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}