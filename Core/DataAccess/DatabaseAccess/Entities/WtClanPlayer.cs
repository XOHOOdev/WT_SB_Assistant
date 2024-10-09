using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[PrimaryKey(nameof(PlayerId), nameof(ClanId))]
public partial class WtClanPlayer
{
    public int PlayerId { get; set; }

    public int ClanId { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual WtClan Clan { get; set; } = null!;

    public virtual WtPlayer Player { get; set; } = null!;
}
