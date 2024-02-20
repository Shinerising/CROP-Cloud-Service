using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CROP.API.Data
{
    [Table("Analysis_Shift")]
    [Index(nameof(StationId))]
    public record ShiftData(string StationId, string ShiftName, DateTimeOffset ShiftTime, string FileName, int PlanCount, int CutCount, int CarCount, int WeightSum, int[] WeightDist)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string? PrimaryId { get; set; }
    }
}
