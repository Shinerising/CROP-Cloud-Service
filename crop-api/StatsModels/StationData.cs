using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CROP.API.Data
{
    [Table("Analysis_Station")]
    [Index(nameof(StationId))]
    public record StationData(string StationId, string Name, string FullName, string Section, string Group, string City, string Description, double Longitude, double Latitude)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string? PrimaryId { get; set; }
        public string GraphName { get; set; } = "";
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int MapDirection { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
    }
}
