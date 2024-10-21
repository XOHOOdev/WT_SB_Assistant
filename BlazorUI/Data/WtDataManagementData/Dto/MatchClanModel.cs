namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto
{
    public class MatchClanModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public MatchModel LastMatch { get; set; } = null!;
    }
}
