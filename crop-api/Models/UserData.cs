using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CROP.API.Models
{
    [XmlType("user")]
    public class UserData
    {
        [Key]
        [XmlAttribute("id")]
        public int Id { get; set; }
        [Required]
        [XmlAttribute("name")]
        public string UserName { get; set; } = "";
        [Required]
        [XmlAttribute("password")]
        public string Password { get; set; } = "";
        [XmlAttribute("role")]
        public string Role { get; set; } = "";
    }
    public class UserInput
    {
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
