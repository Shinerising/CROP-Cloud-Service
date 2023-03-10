using System.ComponentModel.DataAnnotations;

namespace CROP.API.Models
{
    public class FileRecord
    {
        public enum FileAction { Upload, Download, Delete };
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; } = 0;
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public int FileSize { get; set; } = 0;
        public DateTime CreateTime { get; set; } = DateTime.MinValue;
        public DateTime UpdateTime { get; set; } = DateTime.MinValue;
        public DateTime SaveTime { get; set; } = DateTime.UtcNow;
        public string Station { get; set; } = "";
        public string Tag { get; set; } = "";
        public FileAction Action { get; set; } = FileAction.Upload;
    }
}
