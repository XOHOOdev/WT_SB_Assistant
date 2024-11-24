namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.ClanDataManagement.Dto
{
    public class MatchClanModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public MatchModel LastMatch { get; set; } = null!;
    }
}
