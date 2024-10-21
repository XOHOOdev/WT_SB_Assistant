using WtSbAssistant.Core.DataAccess.DatabaseAccess.Entities;

namespace WtSbAssistant.BlazorUI.Data.WtDataManagementData.Dto
{
    public class MatchModel
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public List<MatchPlayerModel> Players { get; set; } = [];

        public WtMatchResult Result { get; set; }
    }
}
