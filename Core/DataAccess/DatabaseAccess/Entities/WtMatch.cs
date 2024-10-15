using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public class WtMatch
{
    [Key]
    public int UniqueId { get; set; }

    public int BattleRatingId { get; set; }

    public DateTime MatchStart { get; set; }

    public DateTime MatchEnd { get; set; }

    public WtMatchResult Result { get; set; }

    public virtual WtBattleRating BattleRating { get; set; } = null!;

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();

    public virtual ICollection<WtClanMatch> WtClanMatch { get; set; } = new List<WtClanMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtMatch match &&
               match.BattleRating == BattleRating &&
               (Equals(MatchStart >= match.MatchStart && MatchStart <= match.MatchEnd) ||
                (match.MatchStart >= MatchStart && match.MatchStart <= MatchEnd));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BattleRatingId);
    }
}
