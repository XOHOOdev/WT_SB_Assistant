﻿using Microsoft.AspNetCore.Identity;
using WtSbAssistant.Core.DataAccess.DatabaseAccess;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WtSbAssistant.BlazorUI.Authorization;

public class PermissionService(IServiceScopeFactory serviceScopeFactory) : IPermissionService
{
    public HashSet<string> GetPermissionAsync(string memberId)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext<IdentityUser, ApplicationRole, string>>();

        var roleIds = context.UserRoles.Where(x => x.UserId == memberId).Select(x => x.RoleId);
        return context.US_Permissions.Where(x => x.Roles.Any(y => roleIds.Contains(y.Id))).Select(x => x.Name)
            .ToHashSet();
    }
}