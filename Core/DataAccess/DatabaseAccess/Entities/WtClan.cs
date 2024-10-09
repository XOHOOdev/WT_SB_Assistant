using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtClan
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtClanPlayer> WtClanPlayers { get; set; } = new List<WtClanPlayer>();

    public virtual ICollection<WtClanMatch> WtClanMatch { get; set; } = new List<WtClanMatch>();
}
