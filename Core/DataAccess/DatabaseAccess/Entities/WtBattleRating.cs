using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public class WtBattleRating
{
    [Key]
    public int UniqueId { get; set; }

    [Column(TypeName = "decimal(2,1)")]
    public decimal BattleRating { get; set; }

    public DateTime From { get; set; }

    public DateTime Until { get; set; }

    public virtual ICollection<WtMatch> WtMatches { get; set; } = new List<WtMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtBattleRating battleRating &&
               battleRating.BattleRating.Equals(BattleRating) &&
               battleRating.From.Equals(From) &&
               battleRating.Until.Equals(Until);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BattleRating, From, Until);
    }
}
