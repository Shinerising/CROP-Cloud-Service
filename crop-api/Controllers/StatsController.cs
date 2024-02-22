using CROP.API.Data;
using CROP.API.Models;
using CROP.API.Utility;
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
            //Response.Headers.Append("Cache-Control", "public, max-age=3600");
            var result = await context.Stations.ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("station", Name = "GetStationInformation")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<StationData>> GetStation([FromQuery(Name = "station")] string station)
        {
            if (!await context.StationDatas.AnyAsync(item => item.StationId == station))
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

        [HttpGet("monitor", Name = "GetMonitorList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> GetMonitorList([FromQuery(Name = "station")] string station)
        {
            if (!await context.MonitorDatas.AnyAsync(item => item.StationId == station))
            {
                return NotFound();
            }
            var result = await context.MonitorDatas.Where(item => item.StationId == station).ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("shift/sum", Name = "GetShiftSum")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> GetShiftSum([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] DateTimeOffset startTime, [FromQuery(Name = "end")] DateTimeOffset endTime)
        {
            var data = context.ShiftDatas.Where(item => (station == null || item.StationId == station) && item.ShiftTime >= startTime && item.ShiftTime <= endTime);
            var planCount = await data.SumAsync(item => item.PlanCount);
            var CutCount = await data.SumAsync(item => item.CutCount);
            var CarCount = await data.SumAsync(item => item.CarCount);
            var WeightSum = await data.SumAsync(item => item.WeightSum);
            var result = new ShiftSum(station, startTime, endTime, planCount, CutCount, CarCount, WeightSum);
            return Ok(result);
        }

        [HttpGet("shift/dist", Name = "GetShiftDist")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> GetShiftDist([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] DateTimeOffset startTime, [FromQuery(Name = "end")] DateTimeOffset endTime)
        {
            var data = context.ShiftDatas.Where(item => (station == null || item.StationId == station) && item.ShiftTime >= startTime && item.ShiftTime <= endTime);
            var list = await data.Select(item => new ShiftSum(null, item.ShiftTime, item.ShiftTime, item.PlanCount, item.CutCount, item.CarCount, item.WeightSum)).ToListAsync();
            var result = new ShiftDist(station, startTime, endTime, list);
            return Ok(result);
        }

        [HttpGet("shift/list", Name = "GetShiftList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<DeviceData>>> GetShiftList([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] DateTimeOffset startTime, [FromQuery(Name = "end")] DateTimeOffset endTime)
        {
            var data = context.ShiftDatas.Where(item => (station == null || item.StationId == station) && item.ShiftTime >= startTime && item.ShiftTime <= endTime);
            var list = await data.ToListAsync();
            var result = new ShiftList(station, startTime, endTime, list);
            return Ok(result);
        }

        [HttpGet("system", Name = "GetSystemStatus")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SystemStatus>> GetSystemStatus()
        {
            if (Utils.IsWindows)
            {
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject Win32_Processor | Measure-Object -Property LoadPercentage -Average; Write-Host $CompObject.Average"), out double cpu);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject -Class WIN32_OperatingSystem; Write-Host ((($CompObject.TotalVisibleMemorySize-$CompObject.FreePhysicalMemory)*100)/$CompObject.TotalVisibleMemorySize)"), out double ram);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject -Class Win32_LogicalDisk; Write-Host (($CompObject[0].size-$CompObject[0].freespace)*100/$CompObject[0].size)"), out double disk);
                _ = double.TryParse(await Utils.ExecuteCommand("$CompObject = Get-WmiObject -Class Win32_PerfFormattedData_Tcpip_NetworkInterface -filter 'BytesTotalPersec>0'; Write-Host ($CompObject.BytesTotalPerSec*800/$CompObject.CurrentBandwidth)"), out double network);
                return Ok(new SystemStatus(cpu, ram, disk, network));
            }
            else
            {
                _ = double.TryParse(await Utils.ExecuteCommand("vmstat | sed -n 3p | awk '{ print 100-$15 }'"), out double cpu);
                _ = double.TryParse(await Utils.ExecuteCommand("free | sed -n 2p | awk '{ print $3/$2*100 }'"), out double ram);
                _ = double.TryParse(await Utils.ExecuteCommand("df | sed -n 2p | awk '{ print $3/$2*100 }'"), out double disk);
                _ = double.TryParse(await Utils.ExecuteCommand("ifstat 0.1 1 | sed -n 3p | awk '{ print ($1+$2)*800/1000000 }'"), out double network);
                return Ok(new SystemStatus(cpu, ram, disk, network));
            }
        }
    }
}
