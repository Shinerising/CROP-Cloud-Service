using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CROP.API.Data
{
    [Table("Analysis_Task")]
    [Index(nameof(StationId))]
    public class AnalysisTask
    {
        public enum TaskType
        {
            None,
            Record,
            Alarm,
            Graph,
            Monitor,
            Config
        }
        public enum TaskState
        {
            Default,
            Idle,
            Created,
            Finished,
            Failed,
        }
        public enum TaskPriority
        {
            None,
            Low,
            Normal,
            High,
            Immediate
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? PrimaryId { get; set; }
        public string StationId { get; set; } = string.Empty;
        public TaskType Type { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }
        public string FileName { get; set; } = "";
        public int FileSize { get; set; }
        public DateTimeOffset FileTime { get; set; }
        public string FileHash { get; set; } = "";
        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset HandleTime { get; set; }
        public DateTimeOffset FinishTime { get; set; }
    }
}
