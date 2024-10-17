using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WebAPI.Dto
{
    public class PlayerVehicle
    {
        public string PlayerName { get; set; } = null!;

        public string VehicleName { get; set; } = null!;

        public List<WtVehicle> PossibleMatches { get; set; } = [];

        public WtVehicle BestMatch { get; set; } = null!;
    }
}
