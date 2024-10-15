using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WtPlayer
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtClanPlayer> WtClanPlayers { get; set; } = new List<WtClanPlayer>();

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtPlayer player && Name.Equals(player.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
