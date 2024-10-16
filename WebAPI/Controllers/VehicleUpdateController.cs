using Microsoft.AspNetCore.Mvc;
using WebAPI.DataAccess;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleUpdateController(VehicleDataManager vehicleData, DatabaseManager dataManager) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<int>> StartUpdate()
        {
            var vehicles = await vehicleData.GetAllVehicles();
            return await dataManager.UpdateVehiclesAsync(vehicles);
        }
    }
}
