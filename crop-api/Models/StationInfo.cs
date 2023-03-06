using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CROP.API.Models {
    public class StationInfo 
    {
        [Key]
        public int Id { get; set; } = 0;
        [Indexed]
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string FullName { get; set; } = "";
        [AllowNull]
        public string Section { get; set; }
        [AllowNull]
        public string Group { get; set; }
        [AllowNull]
        public string City { get; set; }
        [AllowNull]
        public string Description { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;

    }
}
