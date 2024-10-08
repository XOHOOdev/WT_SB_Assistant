using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess;

//add-migration InitialMigration -output DataAccess\DatabaseAccess\Migrations

public class ApplicationDbContext<TUser, TRole, TKey>(
    DbContextOptions<ApplicationDbContext<TUser, TRole, TKey>> options)
    : IdentityDbContext<
        TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
        IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>(options)
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    public DbSet<Permission> US_Permissions { get; set; }

    public DbSet<Configuration> CF_Configurations { get; set; }

    public DbSet<LogMessage> LG_LogMessages { get; set; }
}