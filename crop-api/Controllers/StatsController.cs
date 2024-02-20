using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for file.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SecurityController"/> class.
    /// </remarks>
    /// <param name="configuration">The configuration.</param>
    /// <param name="context">The context.</param>
    [Authorize]
    [ApiController]
    [Route("api/stats")]
    public class StatsController(IConfiguration configuration, PostgresDbContext context) : ControllerBase
    {
        [HttpGet("device", Name = "GetDeviceList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> Get([FromQuery(Name = "station")] string station)
        {
            if (!await context.DeviceDatas.AnyAsync(item => item.StationId == station))
            {
                return NotFound();
            }
            var result = await context.DeviceDatas.Where(item => item.StationId == station).ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }
    }
}
