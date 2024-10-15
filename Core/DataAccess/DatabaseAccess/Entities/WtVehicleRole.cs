using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WtVehicleRole
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtVehicle> WtVehicles { get; set; } = new List<WtVehicle>();

    public override bool Equals(object? obj)
    {
        return obj is WtVehicleRole vehicleRole && Name.Equals(vehicleRole.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
