using Microsoft.AspNetCore.Mvc;
using WebAPI.Dto;
using WebAPI.Parser;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WtLogController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<WtLog>> PostWtLog(WtLog log)
        {
            var dmos = WtLogParser.ParseLog(log);
            return CreatedAtAction(nameof(PostWtLog), null, log);
        }
    }
}
