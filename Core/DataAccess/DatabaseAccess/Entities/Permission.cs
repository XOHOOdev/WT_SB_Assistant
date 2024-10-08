namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public class Permission
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;

    public virtual List<ApplicationRole> Roles { get; } = [];
}