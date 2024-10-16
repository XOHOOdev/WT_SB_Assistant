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

    public DbSet<WtBattleRating> WT_BattleRatings { get; set; }

    public DbSet<WtClan> WT_Clans { get; set; }

    public DbSet<WtClanPlayer> WT_ClanPlayers { get; set; }

    public DbSet<WtMatch> WT_Matches { get; set; }


    public DbSet<WtClanMatch> WT_ClanMatch { get; set; }

    public DbSet<WtNation> WT_Nations { get; set; }

    public DbSet<WtPlayer> WT_Players { get; set; }

    public DbSet<WtVehicle> WT_Vehicles { get; set; }

    public DbSet<WtVehiclePlayerMatch> WT_VehiclePlayerMatches { get; set; }

    public DbSet<WtVehicleVehicleType> WT_VehicleVehicleTypes { get; set; }

    public DbSet<WtVehicleType> WT_VehicleTypes { get; set; }

    public DbSet<WtVehicleBattleRating> WT_VehicleBattleRatings { get; set; }
}