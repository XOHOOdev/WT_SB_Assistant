using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebAPI.DataAccess;
using WtSbAssistant.Core.Dto;
using WtSbAssistant.Core.Logger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WtLogController(WtSbAssistantLogger logger, DatabaseManager database) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<WtLog>> PostWtLog(WtLog[] logs)
        {
            foreach (var log in logs)
            {
                logger.LogVerbose($"Received logs from {log.Time.ToString(CultureInfo.InvariantCulture)}");

                await database.InsertDataAsync(log);
            }

            return CreatedAtAction(nameof(PostWtLog), null, logs);
        }
    }
}
