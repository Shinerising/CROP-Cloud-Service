using CROP.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CROP.API.Data
{
    public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
    {
        public DbSet<UserData> Users { get; set; } = null!;
        public DbSet<FileRecord> FileRecords { get; set; } = null!;
        public DbSet<TagRecord> TagRecords { get; set; } = null!;
        public DbSet<StationInfo> Stations { get; set; } = null!;
        public DbSet<AnalysisTask> Tasks { get; set; } = null!;
        public DbSet<StationData> StationDatas { get; set; } = null!;
        public DbSet<DeviceData> DeviceDatas { get; set; } = null!;
        public DbSet<MonitorData> MonitorDatas { get; set; } = null!;
        public DbSet<ShiftData> ShiftDatas { get; set; } = null!;
    }
}
