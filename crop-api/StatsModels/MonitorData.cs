using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CROP.API.Data
{
    [Table("Analysis_Monitor")]
    [Index(nameof(StationId))]
    public record MonitorData(string StationId, string DeviceId, string DeviceType, string Name, string Direction, string Front, string Rear_Normal, string Rear_Reverse, string Length, string FrontLength)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string? PrimaryId { get; set; }
    }
}
