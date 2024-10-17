namespace WebAPI.Dto
{
    public class PlayerKd
    {
        public string Name { get; set; } = null!;

        public string VehicleName { get; set; } = null!;

        public int Kills { get; set; }

        public int Deaths { get; set; }
    }
}
