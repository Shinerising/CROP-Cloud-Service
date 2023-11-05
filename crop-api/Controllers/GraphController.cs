using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;
using System.Buffers.Text;
using System.IO.Compression;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CROP.API.Controllers {
    /// <summary>
    /// Controller for graph data.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class GraphController : ControllerBase
    {
        private readonly RedisCollection<GraphData> _graph;
        private readonly PostgresDbContext _context;
        private readonly RedisCollection<GraphDataRealTime> _graphRealTime;
        public GraphController(RedisConnectionProvider provider, PostgresDbContext context)
        {
            _graph = (RedisCollection<GraphData>)provider.RedisCollection<GraphData>();
            _graphRealTime = (RedisCollection<GraphDataRealTime>)provider.RedisCollection<GraphDataRealTime>();
            _context = context;
        }

        [HttpGet("graph", Name = "GetGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<GraphData>> Get([FromQuery(Name = "station")] string station)
        {
            if (!await _graphRealTime.AnyAsync(item => item.Station == station))
            {
                return NotFound();
            }
            var result = await _graphRealTime.FirstAsync(item => item.Station == station);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Gets all graph data.
        /// </summary>
        /// <param name="station">The station to get the data for.</param>
        [HttpGet("graph/all", Name = "GetAllGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<GraphData>>> GetAll([FromQuery(Name = "station")] string station)
        {
            var result = await _graph.Where(item => item.Station == station).ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Inserts a new graph data.
        /// </summary>
        [HttpPost("graph", Name = "PutGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Put([FromBody] GraphData data)
        {
            if (!_context.Stations.Any(_station => _station.Id == data.Station))
            {
                return Forbid();
            }

            await _graph.InsertAsync(data, TimeSpan.FromSeconds(3600));

            if (await _graphRealTime.AnyAsync(item => item.Station == data.Station))
            {
                await _graphRealTime.UpdateAsync(new GraphDataRealTime()
                {
                    Station = data.Station,
                    Version = data.Version,
                    Data = data.Data,
                    Time = data.Time
                });
            }
            else
            {
                await _graphRealTime.InsertAsync(new GraphDataRealTime()
                {
                    Station = data.Station,
                    Version = data.Version,
                    Data = data.Data,
                    Time = data.Time
                });
            }
            return Ok();
        }

        [HttpDelete("graph", Name = "DeleteGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete([FromQuery(Name = "station")] string station)
        {
            if (await _graphRealTime.AnyAsync(item => item.Station == station))
            {
                await _graphRealTime.DeleteAsync(await _graphRealTime.FirstAsync(item => item.Station == station));
            }
            return Ok();
        }

        [HttpGet("graph/simple", Name = "GetGraphDecompressed")]
        public async Task<ActionResult<string>> GetGraphDecompressed([FromQuery(Name = "station")] string station)
        {
            if (Request.Headers["CROP-PATH"] != "graph/simple")
            {
                return Forbid();
            }

            if (!await _graphRealTime.AnyAsync(item => item.Station == station))
            {
                return NotFound();
            }

            var result = await _graphRealTime.FirstAsync(item => item.Station == station);
            if (result == null)
            {
                return NotFound();
            }

            string text = $"{result.Time.ToUnixTimeSeconds}\n{result.Data}";

            return Ok(text);
        }
    }
}
