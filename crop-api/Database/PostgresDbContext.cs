using CROP.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CROP.API.Data
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {
        }
        public DbSet<UserData> Users { get; set; } = null!;
        public DbSet<FileRecord> FileRecords { get; set; } = null!;
    }
}
