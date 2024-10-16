using Newtonsoft.Json;

namespace WebAPI.Dto;

public class ApiVehicleImages
{
    [JsonProperty(PropertyName = "image")]
    public string Image { get; set; } = null!;

    [JsonProperty(PropertyName = "techtree")]
    public string TechTree { get; set; } = null!;
}