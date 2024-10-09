using Newtonsoft.Json;

namespace WTBattleExtractor.Dto.WtLocal
{
    public class WtLocalRoot
    {
        [JsonProperty(PropertyName = "events")]
        public List<object> Events { get; set; } = [];

        [JsonProperty(PropertyName = "damage")]
        public List<Damage> Damage { get; set; } = [];

        [JsonIgnore]
        public DateTime Time { get; set; }
    }
}
