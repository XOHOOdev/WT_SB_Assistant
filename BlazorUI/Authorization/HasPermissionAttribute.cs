using Microsoft.AspNetCore.Authorization;

namespace Sparta.BlazorUI.Authorization;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission);