using Microsoft.AspNetCore.Identity;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole(string name)
    {
        Name = name;
        NormalizedName = name.ToUpper();
    }

    public virtual List<Permission> Permissions { get; } = [];
}