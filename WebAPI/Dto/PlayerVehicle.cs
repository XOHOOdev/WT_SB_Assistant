using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WebAPI.Dto
{
    public class PlayerVehicle
    {
        public string VehicleName { get; set; } = null!;

        public List<WtVehicle> PossibleMatches { get; set; } = [];

        public WtBattleAction BattleAction { get; set; } = null!;
    }
}
