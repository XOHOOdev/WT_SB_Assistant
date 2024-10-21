namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto
{
    public class ClanModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public List<ClanPlayerModel> PlayerModels { get; set; } = [];
    }
}
