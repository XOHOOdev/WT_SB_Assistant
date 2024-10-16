using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(BattleRating), IsUnique = true)]
public class WtVehicleBattleRating
{
    [Key]
    public int UniqueId { get; set; }


    [Column(TypeName = "decimal(3,1)")]
    public decimal BattleRating { get; set; }

    public virtual ICollection<WtVehicle> Vehicles { get; set; } = new List<WtVehicle>();

    public override bool Equals(object? obj)
    {
        return obj is WtVehicleBattleRating vehicleBattleRating && BattleRating.Equals(vehicleBattleRating.BattleRating);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BattleRating);
    }
}
