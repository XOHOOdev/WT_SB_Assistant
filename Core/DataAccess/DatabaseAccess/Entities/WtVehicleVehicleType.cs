using Microsoft.EntityFrameworkCore;

namespace WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities
{
    [PrimaryKey(nameof(VehicleId), nameof(VehicleTypeId))]
    public class WtVehicleVehicleType
    {
        public int VehicleId { get; set; }

        public int VehicleTypeId { get; set; }

        public virtual WtVehicle Vehicle { get; set; } = null!;

        public virtual WtVehicleType VehicleType { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return obj is WtClanPlayer cp && cp.PlayerId.Equals(VehicleId) && cp.ClanId.Equals(VehicleTypeId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VehicleId, VehicleTypeId);
        }
    }
}
