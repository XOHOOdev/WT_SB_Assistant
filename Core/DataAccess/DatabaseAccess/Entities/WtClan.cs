using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WtClan
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtClanPlayer> WtClanPlayers { get; set; } = new List<WtClanPlayer>();

    public virtual ICollection<WtClanMatch> WtClanMatch { get; set; } = new List<WtClanMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtClan clan && Name.Equals(clan.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
