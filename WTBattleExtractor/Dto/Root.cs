using Newtonsoft.Json;

namespace WTBattleExtractor.Dto
{
    public class Root
    {
        [JsonProperty(PropertyName = "events")]
        public List<object> Events { get; set; } = [];

        [JsonProperty(PropertyName = "damage")]
        public List<Damage> Damage { get; set; } = [];

        [JsonIgnore]
        public DateTime Received { get; set; }
    }
}
