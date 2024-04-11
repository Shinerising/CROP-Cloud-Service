using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM;
using Redis.OM.Searching;
using System.IO.Compression;
using System.Text;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for graph data.
    /// </summary>
    [ApiController]
    [Route("api/graph")]
    public class GraphController(RedisConnectionProvider provider, PostgresDbContext context) : ControllerBase
    {
        private readonly RedisCollection<GraphData> _graph = (RedisCollection<GraphData>)provider.RedisCollection<GraphData>();
        private readonly RedisCollection<GraphDataRealTime> _graphRealTime = (RedisCollection<GraphDataRealTime>)provider.RedisCollection<GraphDataRealTime>();
        private readonly RedisCollection<GraphDataSimple> _graphSimple = (RedisCollection<GraphDataSimple>)provider.RedisCollection<GraphDataSimple>();
        private readonly RedisCollection<AlarmData> _alarm = (RedisCollection<AlarmData>)provider.RedisCollection<AlarmData>();

        [HttpGet("", Name = "GetGraph")]
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
        [HttpGet("all", Name = "GetAllGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<GraphData>>> GetAll([FromQuery(Name = "station")] string station)
        {
            var result = await _graph.Where(item => item.Station == station).ToListAsync();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("status", Name = "GetStatus")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<GraphStatus>>> GetStatus([FromQuery(Name = "station")] string? station)
        {
            if (station == null)
            {
                var result = await _graphRealTime.Select(item => new GraphStatus(item.Station, DateTimeOffset.Now - item.SaveTime < TimeSpan.FromSeconds(10), item.Time, item.SaveTime)).ToListAsync();
                return result == null ? NotFound() : Ok(result);
            }
            else
            {
                var result = await _graphRealTime.Where(item => item.Station == station).Select(item => new GraphStatus(item.Station, DateTimeOffset.Now - item.SaveTime < TimeSpan.FromSeconds(10), item.Time, item.SaveTime)).ToListAsync();
                return result == null ? NotFound() : Ok(result);
            }
        }

        /// <summary>
        /// Gets alarm data.
        /// </summary>
        /// <param name="station">The station to get the data for.</param>
        [HttpGet("alarm", Name = "GetAlarmData")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<AlarmData>>> GetAlarm([FromQuery(Name = "station")] string? station)
        {
            if (station == null)
            {
                var result = await _alarm.OrderBy(item => item.Time).TakeLast(200).ToListAsync();
                return result == null ? NotFound() : Ok(result);
            }
            else
            {
                var result = await _alarm.Where(item => item.Station == station).OrderBy(item => item.Time).TakeLast(200).ToListAsync();
                return result == null ? NotFound() : Ok(result);
            }
        }

        /// <summary>
        /// Inserts a new graph data.
        /// </summary>
        [HttpPost("", Name = "PutGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Put([FromBody] GraphData data)
        {
            if (!context.Stations.Any(_station => _station.Id == data.Station))
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

            await ExtractGraphData(data);

            return Ok();
        }

        private async Task<bool> ExtractGraphData(GraphData data)
        {
            string base64 = data.Data;
            byte[]? buffer = Convert.FromBase64String(base64);
            int length = (buffer[6] << 8) + buffer[5] - 2;
            int offset = 9;

            buffer = await DecompressData(buffer, offset, length);
            if (buffer == null)
            {
                return false;
            }
            var graphLength = BitConverter.ToUInt16(buffer, 0);
            var graphBuffer = new byte[graphLength];
            var boardLength = BitConverter.ToUInt16(buffer, 2 + graphLength);
            var alarmLength = BitConverter.ToUInt16(buffer, 4 + graphLength + boardLength);
            var alarmBuffer = new byte[alarmLength];
            Buffer.BlockCopy(buffer, 2, graphBuffer, 0, graphLength);
            Buffer.BlockCopy(buffer, 6 + graphLength + boardLength, alarmBuffer, 0, alarmLength);

            string graphBase64 = Convert.ToBase64String(graphBuffer);

            if (await _graphSimple.AnyAsync(item => item.Station == data.Station))
            {
                await _graphSimple.UpdateAsync(new GraphDataSimple()
                {
                    Station = data.Station,
                    Version = data.Version,
                    Data = graphBase64,
                    Time = data.Time
                });
            }
            else
            {
                await _graphSimple.InsertAsync(new GraphDataSimple()
                {
                    Station = data.Station,
                    Version = data.Version,
                    Data = graphBase64,
                    Time = data.Time
                });
            }

            using MemoryStream stream = new(alarmBuffer);
            using StreamReader sr = new(stream, Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                string? text = (await sr.ReadLineAsync())?.Trim();
                if (text != null)
                {
                    await _alarm.InsertAsync(new AlarmData()
                    {
                        Station = data.Station,
                        Version = data.Version,
                        Data = text,
                        Time = data.Time
                    }, TimeSpan.FromSeconds(3600 * 4));
                }
            }

            return true;
        }
        private static async Task<byte[]?> DecompressData(byte[] data, int offset, int count)
        {
            using MemoryStream outputStream = new();
            using MemoryStream inputStream = new(data, offset, count);
            using DeflateStream deflateStream = new(inputStream, CompressionMode.Decompress);
            try
            {
                await deflateStream.CopyToAsync(outputStream);
                return outputStream.ToArray();
            }
            catch
            {
                return null;
            }
        }


        [HttpDelete("", Name = "DeleteGraph")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete([FromQuery(Name = "station")] string station)
        {
            if (await _graphRealTime.AnyAsync(item => item.Station == station))
            {
                await _graphRealTime.DeleteAsync(await _graphRealTime.FirstAsync(item => item.Station == station));
            }
            return Ok();
        }

        [HttpGet("simple", Name = "GetGraphDecompressed")]
        public async Task<ActionResult<string>> GetGraphDecompressed([FromQuery(Name = "station")] string station)
        {
            if (Request.Headers["CROP-PATH"] != "graph/simple")
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(Request.Headers["CROP-TEST"]))
            {
                byte[] data = new byte[10000];
                Random rnd = new();
                rnd.NextBytes(data);
                string base64 = Convert.ToBase64String(data);
                return Ok($"{DateTimeOffset.Now.ToUnixTimeSeconds()}\n{base64}");
            }

            if (!await _graphSimple.AnyAsync(item => item.Station == station))
            {
                return NotFound();
            }

            var result = await _graphSimple.FirstAsync(item => item.Station == station);
            if (result == null)
            {
                return NotFound();
            }

            string text = $"{result.Time.ToUnixTimeSeconds()}\n{result.Data}";

            return Ok(text);
        }
    }
}
