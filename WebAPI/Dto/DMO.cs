namespace WTBattleExtractor.Dto
{
    public class DMO
    {
        public string Player1 { get; set; } = null!;

        public string Vehicle1 { get; set; } = null!;

        public string Player2 { get; set; } = null!;

        public string Vehicle2 { get; set; } = null!;

        public string Action { get; set; } = null!;

        public DateTime Time { get; set; }
    }
}
