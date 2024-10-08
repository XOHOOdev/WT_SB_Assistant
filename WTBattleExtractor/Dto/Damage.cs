using Newtonsoft.Json;

namespace WTBattleExtractor.Dto;

public class Damage
{
    [JsonProperty(PropertyName = "id")]
    public int Id { get; set; }

    [JsonProperty(PropertyName = "msg")]
    public string Msg { get; set; } = null!;

    [JsonProperty(PropertyName = "sender")]
    public string Sender { get; set; } = null!;

    [JsonProperty(PropertyName = "enemy")]
    public bool Enemy { get; set; }

    [JsonProperty(PropertyName = "mode")]
    public string Mode { get; set; } = null!;

    [JsonProperty(PropertyName = "time")]
    public int Time { get; set; }
}