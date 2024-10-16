using Newtonsoft.Json;

namespace WebAPI.Dto
{
    public class ApiVehicle
    {
        [JsonProperty(PropertyName = "identifier")]
        public string Identifier { get; set; } = null!;

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; } = null!;

        [JsonProperty(PropertyName = "vehicle_type")]
        public string VehicleType { get; set; } = null!;

        [JsonProperty(PropertyName = "vehicle_sub_types")]
        public List<string> VehicleSubTypes { get; set; } = [];

        [JsonProperty(PropertyName = "era")]
        public int Era { get; set; }

        [JsonProperty(PropertyName = "arcade_br")]
        public double ArcadeBr { get; set; }

        [JsonProperty(PropertyName = "realistic_br")]
        public double RealisticBr { get; set; }

        [JsonProperty(PropertyName = "realistic_ground_br")]
        public double RealisticGroundBr { get; set; }

        [JsonProperty(PropertyName = "simulator_br")]
        public double SimulatorBr { get; set; }

        [JsonProperty(PropertyName = "simulator_ground_br")]
        public double SimulatorGroundBr { get; set; }

        [JsonProperty(PropertyName = "event")]
        public object? Event { get; set; }

        [JsonProperty(PropertyName = "release_date")]
        public string ReleaseDate { get; set; } = null!;

        [JsonProperty(PropertyName = "is_premium")]
        public bool IsPremium { get; set; }

        [JsonProperty(PropertyName = "is_pack")]
        public bool IsPack { get; set; }

        [JsonProperty(PropertyName = "on_marketplace")]
        public bool OnMarketplace { get; set; }

        [JsonProperty(PropertyName = "squadron_vehicle")]
        public bool SquadronVehicle { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "req_exp")]
        public int ReqExp { get; set; }

        [JsonProperty(PropertyName = "ge_cost")]
        public int GeCost { get; set; }

        [JsonProperty(PropertyName = "sl_mul_arcade")]
        public double SlMulArcade { get; set; }

        [JsonProperty(PropertyName = "sl_mul_realistic")]
        public double SlMulRealistic { get; set; }

        [JsonProperty(PropertyName = "sl_mul_simulator")]
        public double SlMulSimulator { get; set; }

        [JsonProperty(PropertyName = "exp_mul")]
        public double ExpMul { get; set; }

        [JsonProperty(PropertyName = "crew_total_count")]
        public int CrewTotalCount { get; set; }

        [JsonProperty(PropertyName = "visibility")]
        public int Visibility { get; set; }

        [JsonProperty(PropertyName = "hull_armor")]
        public List<double> HullArmor { get; set; } = [];

        [JsonProperty(PropertyName = "turret_armor")]
        public List<double> TurretArmor { get; set; } = [];

        [JsonProperty(PropertyName = "images")]
        public ApiVehicleImages Images { get; set; } = null!;
    }
}
