namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto
{
    public class VehicleModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Matches { get; set; }

        public int GroundKills { get; set; }

        public int AirKills { get; set; }

        public int Deaths { get; set; }
    }
}
