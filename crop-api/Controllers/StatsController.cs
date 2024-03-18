using CROP.API.Data;
using CROP.API.Models;
using CROP.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for file.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SecurityController"/> class.
    /// </remarks>
    /// <param name="configuration">The configuration.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="context">The context.</param>
    [Authorize]
    [ApiController]
    [Route("api/stats")]
    public class StatsController(IConfiguration configuration, RedisConnectionProvider provider, PostgresDbContext context) : ControllerBase
    {
        private readonly RedisCollection<SystemStatus> _systemStatus = (RedisCollection<SystemStatus>)provider.RedisCollection<SystemStatus>();
        private readonly RedisCollection<SystemReport> _systemReport = (RedisCollection<SystemReport>)provider.RedisCollection<SystemReport>();

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

        [HttpGet("station", Name = "GetStation")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<StationData>> GetStation([FromQuery(Name = "station")] string station)
        {
            if (!await context.Stations.AnyAsync())
            {
                return NotFound();
            }
            var result = await context.Stations.FirstOrDefaultAsync(item => item.Id == station);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("station/data", Name = "GetStationInformation")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<StationData>> GetStationData([FromQuery(Name = "station")] string station)
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
        public async Task<ActionResult<List<MonitorData>>> GetMonitorList([FromQuery(Name = "station")] string station)
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
        public async Task<ActionResult<List<ShiftSum>>> GetShiftSum([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] int start, [FromQuery(Name = "end")] int end)
        {
            var startTime = DateTimeOffset.FromUnixTimeSeconds(start);
            var endTime = DateTimeOffset.FromUnixTimeSeconds(end);
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
        public async Task<ActionResult<List<ShiftDist>>> GetShiftDist([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] int start, [FromQuery(Name = "end")] int end)
        {
            var startTime = DateTimeOffset.FromUnixTimeSeconds(start);
            var endTime = DateTimeOffset.FromUnixTimeSeconds(end);
            var data = context.ShiftDatas.Where(item => (station == null || item.StationId == station) && item.ShiftTime >= startTime && item.ShiftTime <= endTime);
            var list = await data.Select(item => new ShiftSum(null, item.ShiftTime, item.ShiftTime, item.PlanCount, item.CutCount, item.CarCount, item.WeightSum)).ToListAsync();
            var result = new ShiftDist(station, startTime, endTime, list);
            return Ok(result);
        }

        [HttpGet("shift/list", Name = "GetShiftList")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<ShiftList>>> GetShiftList([FromQuery(Name = "station")] string? station, [FromQuery(Name = "start")] int start, [FromQuery(Name = "end")] int end)
        {
            var startTime = DateTimeOffset.FromUnixTimeSeconds(start);
            var endTime = DateTimeOffset.FromUnixTimeSeconds(end);
            var data = context.ShiftDatas.Where(item => (station == null || item.StationId == station) && item.ShiftTime >= startTime && item.ShiftTime <= endTime);
            var list = await data.ToListAsync();
            var result = new ShiftList(station, startTime, endTime, list);
            return Ok(result);
        }

        [HttpGet("system", Name = "GetSystemStatus")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SystemStatus>> GetSystemStatus()
        {
            if (!await _systemStatus.AnyAsync())
            {
                return NotFound();
            }
            var systemStatus = await _systemStatus.FirstAsync();
            return systemStatus == null ? NotFound() : Ok(systemStatus);
        }

        [HttpGet("report", Name = "GetSystemReport")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SystemReport>> GetSystemReport()
        {
            if (!await _systemReport.AnyAsync())
            {
                return NotFound();
            }
            var systemReport = await _systemReport.FirstAsync();
            return systemReport == null ? NotFound() : Ok(systemReport);
        }
    }
}
