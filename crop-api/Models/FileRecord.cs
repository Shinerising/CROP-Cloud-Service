using System.ComponentModel.DataAnnotations;

namespace CROP.API.Models
{
    /// <summary>
    /// 日志文件存储记录
    /// </summary>
    public class FileRecord
    {
        /// <summary>
        /// 文件操作类型
        /// </summary>
        public enum FileAction { Upload, Download, Delete };
        /// <summary>
        /// 文件记录ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; } = 0;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; } = "";
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; } = "";
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; } = 0;
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 文件更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 文件保存时间
        /// </summary>
        public DateTime SaveTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// 地点名称
        /// </summary>
        public string Station { get; set; } = "";
        /// <summary>
        /// 文件标签
        /// </summary>
        public string Tag { get; set; } = "";
        /// <summary>
        /// 文件操作类型
        /// </summary>
        public FileAction Action { get; set; } = FileAction.Upload;
    }
}
