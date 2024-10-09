using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtBattleRating
{
    [Key]
    public int UniqueId { get; set; }

    [Column(TypeName = "decimal(2,1)")]
    public decimal BattleRating { get; set; }

    public DateTime From { get; set; }

    public DateTime Until { get; set; }

    public virtual ICollection<WtMatch> WtMatches { get; set; } = new List<WtMatch>();
}
