using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CROP.API.Models
{
    [Index(nameof(StationId))]
    public class ShiftData
    {
        public string StationId { get; set; } = "";
        public string FileName { get; set; } = "";
        public DateTimeOffset StartTime { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset EndTime { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset FileTime { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset RefreshTime { get; set; } = DateTimeOffset.UtcNow;
        public int PlanCount { get; set; } = 0;
        public int CutCount { get; set; } = 0;
        public int WeightTotal { get; set; } = 0;
        public int CarTotal { get; set; } = 0;
        [Column(TypeName = "integer[]")]
        public int[] WeightDistrib { get; set; } = new int[4];
        [Column(TypeName = "integer[]")]
        public int[] CarDistrib { get; set; } = new int[4];
    }
}
