namespace WTBattleExtractor.Dto.WtSbAssistant
{
    public class WtLog
    {
        public List<WtLogItem> Logs { get; set; } = [];

        public DateTime Time { get; set; } = DateTime.MaxValue;

        public string? Result { get; set; }
    }
}
