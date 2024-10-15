using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities
{
    [PrimaryKey(nameof(ClanId), nameof(MatchId))]
    public class WtClanMatch
    {
        public int ClanId { get; set; }

        public int MatchId { get; set; }

        public virtual WtClan Clan { get; set; } = null!;

        public virtual WtMatch Match { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return obj is WtClanMatch cm && cm.ClanId.Equals(ClanId) && cm.MatchId.Equals(MatchId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClanId, MatchId);
        }
    }
}
