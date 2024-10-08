namespace Sparta.BlazorUI.Data.UserManagementData;

public class RolePermissionModel : IPermissionModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool Selected { get; set; }
}