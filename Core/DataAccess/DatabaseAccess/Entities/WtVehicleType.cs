using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WtVehicleType
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtVehicleVehicleType> VehicleTypes { get; set; } = new List<WtVehicleVehicleType>();

    public override bool Equals(object? obj)
    {
        return obj is WtVehicleType vehicleType && Name.Equals(vehicleType.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
