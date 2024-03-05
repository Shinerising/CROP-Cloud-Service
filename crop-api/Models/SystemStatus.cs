using Redis.OM.Modeling;

namespace CROP.API.Models
{
    [Document]
    public class SystemStatus
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; init; } = "SystemStatus";
        public DateTimeOffset Time { get; init; } = DateTimeOffset.Now;
        public double CpuUsage { get; init; } = 0;
        public double RamUsage { get; init; } = 0;
        public double DiskUsage { get; init; } = 0;
        public double NetworkUsage { get; init; } = 0;
    }

    [Document]
    public class SystemReport
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; init; } = "SystemReport";
        public DateTimeOffset Time { get; init; } = DateTimeOffset.Now;
        public string Text { get; init; } = "";
    }
}
