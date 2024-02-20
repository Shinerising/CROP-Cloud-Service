using Redis.OM.Modeling;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CROP.API.Models
{
    [XmlType("station")]
    public class StationInfo
    {
        [Indexed]
        [Required]
        [XmlAttribute("id")]
        public string Id { get; set; } = "";
        [Required]
        [XmlAttribute("name")]
        public string Name { get; set; } = "";
        [AllowNull]
        [XmlAttribute("fullname")]
        public string FullName { get; set; } = "";
        [AllowNull]
        [XmlAttribute("section")]
        public string Section { get; set; } = "";
        [AllowNull]
        [XmlAttribute("group")]
        public string Group { get; set; } = "";
        [AllowNull]
        [XmlAttribute("city")]
        public string City { get; set; } = "";
        [AllowNull]
        [XmlAttribute("description")]
        public string Description { get; set; } = "";
        [AllowNull]
        [XmlAttribute("long")]
        public double Longitude { get; set; } = 0;
        [AllowNull]
        [XmlAttribute("lati")]
        public double Latitude { get; set; } = 0;
        [AllowNull]
        [XmlAttribute("record")]
        [JsonIgnore]
        public string RecordPath { get; set; } = "";
        [AllowNull]
        [XmlAttribute("alarm")]
        [JsonIgnore]
        public string AlarmPath { get; set; } = "";
        [AllowNull]
        [XmlAttribute("monitor")]
        [JsonIgnore]
        public string MonitorPath { get; set; } = "";
        [AllowNull]
        [XmlAttribute("graph")]
        [JsonIgnore]
        public string GraphPath { get; set; } = "";

        [XmlAttribute("disable")]
        [JsonIgnore]
        public bool IsDisabled { get; set; }
    }
}
