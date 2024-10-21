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
            var fails = 0;

            foreach (var log in logs)
            {
                logger.LogVerbose($"Received logs from {log.Time.ToString(CultureInfo.InvariantCulture)}");

                var result = await database.InsertDataAsync(log);
                if (!result.Success) fails++;
            }
            if (fails > 0) BadRequest($"{fails} Fails. Partial data might have been inserted.");
            return CreatedAtAction(nameof(PostWtLog), null, logs);
        }
    }
}
