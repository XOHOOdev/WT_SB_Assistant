using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtPlayer
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtClanPlayer> WtClanPlayers { get; set; } = new List<WtClanPlayer>();

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();
}
