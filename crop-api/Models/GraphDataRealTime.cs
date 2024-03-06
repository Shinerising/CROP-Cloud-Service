using Redis.OM.Modeling;

namespace CROP.API.Models
{
    [Document]
    public class GraphDataRealTime
    {
        [RedisIdField]
        [Indexed]
        public string Station { get; set; } = "";
        public string Version { get; set; } = "";
        public DateTimeOffset Time { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset SaveTime { get; set; } = DateTimeOffset.Now;
        public string Data { get; set; } = "";
    }
    public record GraphStatus(string Station,bool IsOnline, DateTimeOffset LatestTime, DateTimeOffset SaveTime);
}
