using System.ComponentModel.DataAnnotations;

namespace CROP.API.Models
{
    public class Config
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
