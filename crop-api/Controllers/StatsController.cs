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
        [HttpGet("", Name = "GetStationList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<StationInfo>>> GetStationList()
        {
            if (!await context.Stations.AnyAsync())
            {
                return NotFound();
            }
            var result = await context.Stations.ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("station", Name = "GetStationInformation")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<StationData>> GetStation([FromQuery(Name = "station")] string station)
        {
            if (!await context.StationDatas.AnyAsync())
            {
                return NotFound();
            }
            var result = await context.StationDatas.FirstOrDefaultAsync(item => item.StationId == station);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("device", Name = "GetDeviceList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> GetDeviceList([FromQuery(Name = "station")] string station)
        {
            if (!await context.DeviceDatas.AnyAsync(item => item.StationId == station))
            {
                return NotFound();
            }
            var result = await context.DeviceDatas.Where(item => item.StationId == station).ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("sytem", Name = "GetSystemStatus")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SystemStatus>> GetSystemStatus()
        {
            double cpu = 0;
            double ram = 0;
            double disk = 0;
            double network = 0;
            return Ok(new SystemStatus(cpu, ram, disk, network));
        }
    }
}
