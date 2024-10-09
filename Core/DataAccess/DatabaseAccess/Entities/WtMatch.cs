using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtMatch
{
    [Key]
    public int UniqueId { get; set; }

    public int BattleRatingId { get; set; }

    public DateTime MatchStart { get; set; }

    public int Result { get; set; }

    public virtual WtBattleRating BattleRating { get; set; } = null!;

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();

    public virtual ICollection<WtClanMatch> WtClanMatch { get; set; } = new List<WtClanMatch>();
}
