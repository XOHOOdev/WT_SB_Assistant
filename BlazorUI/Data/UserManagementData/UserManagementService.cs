using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sparta.BlazorUI.Authorization;
using Sparta.Core.DataAccess.DatabaseAccess;
using Sparta.Core.DataAccess.DatabaseAccess.Entities;

namespace Sparta.BlazorUI.Data.UserManagementData;

[HasPermission(Permissions.Permissions.UserManagement.View)]
public class UserManagementService(ApplicationDbContext<IdentityUser, ApplicationRole, string> context)
{
    public IEnumerable<IdentityUser> GetUsers()
    {
        return context.Users;
    }

    public IEnumerable<ApplicationRole> GetRoles()
    {
        return context.Roles;
    }

    public async Task AddNewRoleAsync(string? name)
    {
        if (name != null && context.Roles.FirstOrDefault(x => x.Name == name) == null)
            await context.AddAsync(new ApplicationRole(name.Trim()));
        context.SaveChanges();
    }

    public void DeleteRole(ApplicationRole role)
    {
        if (role != null) context.Roles.Remove(role);
        context.SaveChanges();
    }

    public void DeleteUser(IdentityUser user)
    {
        if (user != null) context.Users.Remove(user);
        context.SaveChanges();
    }

    public RolePermission GetPermissions(ApplicationRole role)
    {
        return new RolePermission
        {
            ApplicationRole = role,
            RolePermissions = GetPermissions(typeof(Permissions.Permissions).GetNestedTypes(), role),
            DiscordGuilds = context.DC_Guilds.Select(g => new DiscordGuildModel
            {
                Id = g.Id,
                Name = g.Name,
                Selected = g.ApplicationRoles.Contains(role)
            }).ToArray()
        };
    }

    private List<IPermissionModel> GetPermissions(IEnumerable<Type> typePermissions, ApplicationRole role)
    {
        List<IPermissionModel> permissions = new();

        foreach (var permission in typePermissions)
        {
            var subPermissions = new List<IPermissionModel>();

            subPermissions.AddRange(permission.GetFields().Select(x =>
            {
                var dbPermission = context.US_Permissions.First(y => y.Name == (x.GetValue(null) as string ?? ""));
                return new RolePermissionModel
                {
                    Id = dbPermission.Id,
                    Name = x.Name,
                    Selected = role.Permissions.Any(x => x.Id == dbPermission.Id)
                };
            }));

            subPermissions.AddRange(GetPermissions(permission.GetNestedTypes(), role));

            permissions.Add(new BaseRolePermissionModel
            {
                BaseName = permission.Name,
                RolePermissions = subPermissions
            });
        }

        return permissions;
    }

    public void SavePermissions(RolePermission rolePermissionModel)
    {
        var role = context.Roles.FirstOrDefault(x => x.Id == rolePermissionModel.ApplicationRole.Id);
        if (role == null) return;
        SavePermissions(rolePermissionModel.RolePermissions, role);
        SaveGuilds(rolePermissionModel.DiscordGuilds, role);
        context.SaveChanges();
    }

    private void SavePermissions(IList<IPermissionModel> permissions, ApplicationRole role)
    {
        foreach (var permission in permissions)
        {
            if (permission is BaseRolePermissionModel basePermission)
            {
                SavePermissions(basePermission.RolePermissions, role);
                continue;
            }

            if (permission is not RolePermissionModel rolePermission) continue;

            var roleHasPermission = role.Permissions.Any(x => x.Id == rolePermission.Id);
            switch (rolePermission.Selected)
            {
                case true when !roleHasPermission:
                    role.Permissions.Add(context.US_Permissions.First(x => x.Id == rolePermission.Id));
                    break;
                case false when roleHasPermission:
                    {
                        var permissionsToRemove = role.Permissions.FirstOrDefault(x => x.Id == rolePermission.Id);
                        if (permissionsToRemove != null) role.Permissions.Remove(permissionsToRemove);
                        break;
                    }
            }
        }
    }

    private void SaveGuilds(IList<DiscordGuildModel> guilds, ApplicationRole role)
    {
        foreach (var guild in guilds)
        {
            var roleHasGuild = role.DiscordGuilds.Any(g => g.Id == guild.Id);
            switch (guild.Selected)
            {
                case true when !roleHasGuild:
                    role.DiscordGuilds.Add(context.DC_Guilds.First(g => g.Id == guild.Id));
                    break;
                case false when roleHasGuild:
                    var guildToRemove = role.DiscordGuilds.FirstOrDefault(g => g.Id == guild.Id);
                    if (guildToRemove != null) role.DiscordGuilds.Remove(guildToRemove);
                    break;
            }
        }
    }

    public UserRoleModel GetUserRoles(IdentityUser user)
    {
        var roleModels = new List<UserRoleEntryModel>();
        foreach (var role in context.Roles)
        {
            if (role == null) continue;
            var userRolesViewModel = new UserRoleEntryModel
            {
                Name = role.Name ?? "",
                Selected = context.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == role.Id),
                Id = role.Id
            };
            roleModels.Add(userRolesViewModel);
        }

        return new UserRoleModel
        {
            User = user,
            Roles = roleModels
        };
    }

    public void SaveUserRoles(UserRoleModel userRoleModel)
    {
        context.UserRoles.Where(x => x.UserId == userRoleModel.User.Id).ExecuteDelete();
        foreach (var role in userRoleModel.Roles)
            if (role.Selected)
                context.UserRoles.Add(
                    new IdentityUserRole<string> { RoleId = role.Id, UserId = userRoleModel.User.Id });
        context.SaveChanges();
    }

    public UserSteamId GetSteamId(IdentityUser user)
    {
        return new UserSteamId
        {
            User = user,
            SteamId = context.US_SteamIds.FirstOrDefault(x => x.UserId == user.Id)?.SteamId ?? 0
        };
    }

    public void SaveUserSteamId(UserSteamId userSteam)
    {
        var userSteamID = context.US_SteamIds.FirstOrDefault(x => x.UserId == userSteam.User.Id);
        if (userSteamID == null)
        {
            userSteamID = new Sparta.Core.DataAccess.DatabaseAccess.Entities.UserSteamId { UserId = userSteam.User.Id };
            context.US_SteamIds.Add(userSteamID);
        }

        userSteamID.SteamId = userSteam.SteamId;
        context.SaveChanges();
    }
}