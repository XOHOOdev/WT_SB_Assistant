using Sparta.Core.DataAccess.DatabaseAccess.Entities;

namespace Sparta.BlazorUI.Data.UserManagementData;

public class RolePermission
{
    public ApplicationRole ApplicationRole { get; set; } = null!;
    public IList<IPermissionModel> RolePermissions { get; set; } = null!;
    public IList<DiscordGuildModel> DiscordGuilds { get; set; } = null!;
}