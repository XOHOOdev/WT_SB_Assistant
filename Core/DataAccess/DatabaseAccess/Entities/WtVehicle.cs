using System.ComponentModel.DataAnnotations;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

public partial class WtVehicle
{
    [Key]
    public int UniqueId { get; set; }

    public int NationId { get; set; }

    public int RoleId { get; set; }

    public int TypeId { get; set; }

    public virtual WtNation Nation { get; set; } = null!;

    public virtual WtVehicleRole Role { get; set; } = null!;

    public virtual WtVehicleType Type { get; set; } = null!;

    public virtual ICollection<WtVehiclePlayerMatch> WtVehiclePlayerMatches { get; set; } = new List<WtVehiclePlayerMatch>();
}
