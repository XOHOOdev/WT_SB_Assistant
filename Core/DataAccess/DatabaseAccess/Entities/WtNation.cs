using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtNation
{
    [Key]
    public int UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<WtVehicle> WtVehicles { get; set; } = new List<WtVehicle>();
}
