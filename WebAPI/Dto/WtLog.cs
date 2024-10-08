namespace WtSbAssistant.BlazorUI.Controllers.Dto
{
    public class WtLog
    {
        public List<WtLogItem> Logs { get; set; } = [];

        public DateTime Time { get; set; }
    }
}
