namespace Sparta.BlazorUI.Data.UserManagementData;

public class BaseRolePermissionModel : IPermissionModel
{
    public string BaseName { get; set; } = null!;
    public IList<IPermissionModel> RolePermissions { get; set; } = null!;
}