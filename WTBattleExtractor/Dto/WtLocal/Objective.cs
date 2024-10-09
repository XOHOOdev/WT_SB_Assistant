using Newtonsoft.Json;

namespace WTBattleExtractor.Dto.WtLocal;

public class Objective
{
    [JsonProperty(PropertyName = "primary")]
    public bool Primary { get; set; }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; } = null!;

    [JsonProperty(PropertyName = "text")]
    public string Text { get; set; } = null!;
}