namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.ClanDataManagement.Dto
{
    public class ClanPlayerModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public List<VehicleModel> Vehicles { get; set; } = [];
    }
}
