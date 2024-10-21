namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto
{
    public class MatchPlayerModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public VehicleModel Vehicle { get; set; } = null!;
    }
}
