using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Searching;

namespace CROP.API.Controllers {
    /// <summary>
    /// Controller for graph data.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api")]
    public class GraphController : ControllerBase
    {
        private readonly RedisCollection<GraphData> _graph;
        public GraphController(RedisConnectionProvider provider)
        {
            _graph = (RedisCollection<GraphData>)provider.RedisCollection<GraphData>();
        }

        [HttpGet("graph", Name = "GetGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<GraphData>> Get([FromQuery(Name = "station")] string station)
        {
            var result = await _graph.Where(item => item.Station == station).OrderByDescending(item => item.Time).FirstAsync();
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
            await _graph.InsertAsync(data, TimeSpan.FromSeconds(3600));
            return Ok();
        }
    }
}
