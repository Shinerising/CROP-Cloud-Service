using System.ComponentModel.DataAnnotations;

namespace CROP.API.Models
{
    public class StationData
    {
        public string Id { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Section { get; set; } = "";
        public string Group { get; set; } = "";
        public string City { get; set; } = "";
        public string Description { get; set; } = "";
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        public int DeviceCount { get; set; } = 0;
    }
}
