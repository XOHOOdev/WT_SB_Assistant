using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WtVehicle
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public int NationId { get; set; }

    public int RoleId { get; set; }

    public int TypeId { get; set; }

    public virtual WtNation Nation { get; set; } = null!;

    public virtual WtVehicleRole Role { get; set; } = null!;

    public virtual WtVehicleType Type { get; set; } = null!;

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();

    public override bool Equals(object? obj)
    {
        return obj is WtVehicle vehicle && Name.Equals(vehicle.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
