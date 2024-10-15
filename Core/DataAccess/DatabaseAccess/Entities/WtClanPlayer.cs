using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[PrimaryKey(nameof(PlayerId), nameof(ClanId))]
public class WtClanPlayer
{
    public int PlayerId { get; set; }

    public int ClanId { get; set; }

    public DateTime LastSeen { get; set; }

    public virtual WtClan Clan { get; set; } = null!;

    public virtual WtPlayer Player { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        return obj is WtClanPlayer cp && cp.PlayerId.Equals(PlayerId) && cp.ClanId.Equals(ClanId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerId, ClanId);
    }
}
