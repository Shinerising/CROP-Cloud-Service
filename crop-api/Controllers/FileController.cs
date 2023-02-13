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
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PostgresDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        public FileController(IConfiguration configuration, PostgresDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("file/upload")]
        [Authorize]
        public async Task<ActionResult> UploadFile([FromQuery(Name = "station")] string station, [FromForm] FormFile file, [FromForm] DateTime createTime, [FromForm] DateTime updateTime)
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
                Station = station
            };

            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using var stream = System.IO.File.Create(tempFile);
            await file.CopyToAsync(stream);

            var targetFile = Path.Combine(_configuration["File:Database"] ?? "", "Database", station, file.FileName);
            System.IO.File.Move(tempFile, targetFile);
            System.IO.File.SetCreationTimeUtc(tempFile, createTime);
            System.IO.File.SetLastWriteTimeUtc(tempFile, updateTime);

            _context.FileRecords.Add(fileRecord);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Route("file/get")]
        [Authorize]
        public ActionResult<FileRecord> GetFile([FromQuery(Name = "id")] int? id, [FromQuery(Name = "station")] string? station, [FromQuery(Name = "filename")] string? filename)
        {
            var result = id == null ? _context.FileRecords.First(item => item.Station == station && item.FileName == filename) : _context.FileRecords.First(item => item.Id == id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [Route("file/list")]
        [Authorize]
        public ActionResult<List<FileRecord>> GetFileList([FromQuery(Name = "station")] string station)
        {
            if (string.IsNullOrEmpty(station))
            {
                return BadRequest();
            }

            var result = _context.FileRecords.Where(item => item.Station == station).ToList();
            return result == null || result.Count == 0 ? NotFound() : Ok(result);
        }


        [HttpGet]
        [Route("file/download")]
        [Authorize]
        public ActionResult DownloadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "filename")] string filename)
        {
            var targetFile = Path.Combine(_configuration["File:Database"] ?? "", "Database", station, filename);

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