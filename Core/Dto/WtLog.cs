namespace WtSbAssistant.Core.Dto
{
    public class WtLog
    {
        public List<WtLogItem> Logs { get; set; } = [];

        public DateTime Time { get; set; }

        public string? Result { get; set; }
    }
}
