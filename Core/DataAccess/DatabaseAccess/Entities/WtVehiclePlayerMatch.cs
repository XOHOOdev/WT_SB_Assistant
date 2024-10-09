using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[PrimaryKey(nameof(VehicleId), nameof(PlayerId), nameof(MatchId))]
public partial class WtVehiclePlayerMatch
{
    public int VehicleId { get; set; }

    public int PlayerId { get; set; }

    public int MatchId { get; set; }

    public virtual WtMatch Match { get; set; } = null!;

    public virtual WtPlayer Player { get; set; } = null!;

    public virtual WtVehicle Vehicle { get; set; } = null!;
}
