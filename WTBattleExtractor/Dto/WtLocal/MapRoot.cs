using Newtonsoft.Json;

namespace WTBattleExtractor.Dto.WtLocal
{
    public class MapRoot
    {
        [JsonProperty(PropertyName = "grid_size")]
        public List<double> GridSize { get; set; } = [];

        [JsonProperty(PropertyName = "grid_steps")]
        public List<double> GridSteps { get; set; } = [];

        [JsonProperty(PropertyName = "grid_zero")]
        public List<double> GridZero { get; set; } = [];

        [JsonProperty(PropertyName = "hud_type")]
        public int HudType { get; set; }

        [JsonProperty(PropertyName = "map_generation")]
        public int MapGeneration { get; set; }

        [JsonProperty(PropertyName = "map_max")]
        public List<double> MapMax { get; set; } = [];

        [JsonProperty(PropertyName = "map_min")]
        public List<double> MapMin { get; set; } = [];

        [JsonProperty(PropertyName = "valid")]
        public bool Valid { get; set; }
    }
}
