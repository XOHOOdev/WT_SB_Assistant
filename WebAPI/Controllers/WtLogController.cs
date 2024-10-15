using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebAPI.DataAccess;
using WebAPI.Parser;
using WtSbAssistant.Core.Dto;
using WtSbAssistant.Core.Logger;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WtLogController(WtSbAssistantLogger logger, DatabaseManager database) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<WtLog>> PostWtLog(WtLog log)
        {
            logger.LogVerbose($"Received log from {log.Time.ToString(CultureInfo.InvariantCulture)}");
            var dmos = WtLogParser.ParseLog(log);

            await database.InsertDataAsync(dmos, log.Time);

            return CreatedAtAction(nameof(PostWtLog), null, log);
        }
    }
}
