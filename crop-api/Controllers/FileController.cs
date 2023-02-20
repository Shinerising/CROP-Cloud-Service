using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for file.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api")]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PostgresDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="context">The context.</param>
        public FileController(IConfiguration configuration, PostgresDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="station">The station to upload the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        /// <param name="file">The file to upload.</param>
        /// <param name="createTime">The creation time of the file.</param>
        /// <param name="updateTime">The update time of the file.</param>
        [HttpPost]
        [Route("file/upload", Name = "UploadFile")]
        [Authorize]
        public async Task<ActionResult> UploadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromForm] FormFile file, [FromForm] DateTime createTime, [FromForm] DateTime updateTime)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            var fileRecord = new FileRecord
            {
                Id = 0,
                UserId = int.Parse(User.FindFirstValue("Id") ?? "0"),
                FileName = file.FileName,
                FileSize = (int)file.Length,
                ContentType = file.ContentType,
                Action = FileRecord.FileAction.Upload,
                CreateTime = createTime,
                UpdateTime = updateTime,
                SaveTime = DateTime.UtcNow,
                Station = station,
                Tag = tag
            };

            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using var stream = System.IO.File.Create(tempFile);
            await file.CopyToAsync(stream);

            var targetFile = Path.Combine(_configuration["File:Database"] ?? "", station, tag, file.FileName);
            System.IO.Directory.CreateDirectory(Path.Combine(_configuration["File:Database"] ?? "", station, tag));
            System.IO.File.Move(tempFile, targetFile);
            System.IO.File.SetCreationTimeUtc(tempFile, createTime);
            System.IO.File.SetLastWriteTimeUtc(tempFile, updateTime);

            _context.FileRecords.Add(fileRecord);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Get file information.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        /// <param name="filename">The name of the file.</param>
        [HttpGet]
        [Route("file/get", Name = "GetFile")]
        [Authorize]
        public ActionResult<FileRecord> GetFile([FromQuery(Name = "id")] int? id, [FromQuery(Name = "station")] string? station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string? filename)
        {
            var result = id == null ? _context.FileRecords.First(item => item.Station == station && item.Tag == tag && item.FileName == filename) : _context.FileRecords.First(item => item.Id == id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Get file list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("file/list", Name = "GetFileList")]
        [Authorize]
        public ActionResult<List<FileRecord>> GetFileList([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag)
        {
            if (string.IsNullOrEmpty(station))
            {
                return BadRequest();
            }

            var result = _context.FileRecords.Where(item => item.Station == station && item.Tag == tag).ToList();
            return result == null || result.Count == 0 ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Get tag list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        [HttpGet]
        [Route("file/tags", Name = "GetTagList")]
        [Authorize]
        public ActionResult<List<string>> GetTagList([FromQuery(Name = "station")] string station)
        {
            if (string.IsNullOrEmpty(station))
            {
                return BadRequest();
            }

            var result = _context.FileRecords.Where(item => item.Station == station).Select(item => item.Tag).Distinct().ToList();
            result = result.Concat(new string[] { "Alarm", "Record" }).Distinct().ToList();
            return result == null || result.Count == 0 ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("file/download", Name = "DownloadFile")]
        [Authorize]
        public ActionResult DownloadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            var targetFile = Path.Combine(_configuration["File:Database"] ?? "", station, tag, filename);

            if (System.IO.File.Exists(targetFile))
            {
                return File(System.IO.File.OpenRead(targetFile), "application/octet-stream", Path.GetFileName(targetFile));
            }
            else
            {
                return NotFound();
            }
        }
    }
}