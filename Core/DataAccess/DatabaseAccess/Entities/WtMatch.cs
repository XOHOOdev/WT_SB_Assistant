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

    public virtual ICollection<WtBattleAction> WtBattleActions { get; set; } = new List<WtBattleAction>();

    public virtual ICollection<WtClanMatch> WtClanMatches { get; set; } = new List<WtClanMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtMatch match &&
               match.BattleRatingId == BattleRatingId &&
               MatchStart <= match.MatchEnd && match.MatchStart <= MatchEnd; //check for overlap of matches
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BattleRatingId);
    }
}
