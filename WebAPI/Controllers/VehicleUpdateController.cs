using Microsoft.AspNetCore.Mvc;
using WebAPI.DataAccess;
using WtSbAssistant.Core.Dto;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleUpdateController(VehicleDataManager vehicleData, DatabaseManager dataManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<int>>> StartUpdate()
    {
        var vehicles = await vehicleData.GetAllVehicles();
        var vehicleIdentifiers = vehicleData.GetIdentifierTranslation();
        var result = await dataManager.UpdateVehiclesAsync(vehicles, vehicleIdentifiers);
        return Ok(result);
    }
}