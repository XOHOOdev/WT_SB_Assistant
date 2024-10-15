using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

[PrimaryKey(nameof(VehicleId), nameof(PlayerId), nameof(MatchId))]
public class WtVehiclePlayerMatch
{
    public int VehicleId { get; set; }

    public int PlayerId { get; set; }

    public int MatchId { get; set; }

    public int Kills { get; set; }

    public int Deaths { get; set; }

    public virtual WtMatch Match { get; set; } = null!;

    public virtual WtPlayer Player { get; set; } = null!;

    public virtual WtVehicle Vehicle { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        return obj is WtVehiclePlayerMatch vpm &&
               vpm.VehicleId == VehicleId &&
               vpm.PlayerId == PlayerId &&
               vpm.MatchId == MatchId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(VehicleId, PlayerId, MatchId);
    }
}
