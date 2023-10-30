using Redis.OM.Modeling;

namespace CROP.API.Models
{
    [Document(StorageType = StorageType.Json)]
    public class GraphDataRealTime
    {
        [RedisIdField]
        [Indexed]
        public string Station { get; set; } = "";
        public string Version { get; set; } = "";
        public DateTimeOffset Time { get; set; } = DateTimeOffset.MinValue;
        public string Data { get; set; } = "";
    }
}
