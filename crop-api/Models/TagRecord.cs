using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations;

namespace CROP.API.Models {
    public class TagRecord
    {
        [Key]
        public int Id { get; set; }
        [Indexed]
        [Required]
        public string Name { get; set; } = "";
        [Indexed]
        [Required]
        public string Station { get; set; } = "";
        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    }
}
