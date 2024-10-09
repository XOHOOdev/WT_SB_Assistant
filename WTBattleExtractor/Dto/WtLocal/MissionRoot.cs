using Newtonsoft.Json;

namespace WTBattleExtractor.Dto.WtLocal
{
    internal class MissionRoot
    {
        [JsonProperty(PropertyName = "objectives")]
        public List<Objective> Objectives { get; set; } = null!;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;
    }
}
