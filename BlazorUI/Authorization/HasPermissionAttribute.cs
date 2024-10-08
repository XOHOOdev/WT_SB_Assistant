using Microsoft.AspNetCore.Authorization;

namespace WtSbAssistant.BlazorUI.Authorization;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission);