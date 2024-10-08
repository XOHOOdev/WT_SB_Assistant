using Microsoft.AspNetCore.Mvc;
using WebAPI.Parser;
using WtSbAssistant.BlazorUI.Controllers.Dto;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WtLogController : ControllerBase
    {
        [HttpPost("{log}")]
        public async Task<ActionResult<WtLog>> PostWtLog(WtLog log)
        {
            var dmos = WtLogParser.ParseLog(log);
            return CreatedAtAction(nameof(PostWtLog), new { id = log.Time }, log);
        }
    }
}
