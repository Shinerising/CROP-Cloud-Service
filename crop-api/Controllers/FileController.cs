using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.IO;
using System.Security.Claims;
using FileSystem = System.IO.File;

namespace CROP.API.Controllers
{
    /// <summary>
    /// Controller for file.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FileController"/> class.
    /// </remarks>
    /// <param name="configuration">The configuration.</param>
    /// <param name="context">The context.</param>
    [Authorize]
    [ApiController]
    [Route("api/file")]
    public class FileController(IConfiguration configuration, PostgresDbContext context) : ControllerBase
    {
        /// <summary>
        /// Upload file.
        /// </summary>
        /// <param name="station">The station of the file.</param>
        /// <param name="tag">The tag of the file.</param>
        /// <param name="createTime">The creation time of the file.</param>
        /// <param name="updateTime">The update time of the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileSize">The size of the file.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPost]
        [Route("upload", Name = "UploadFile")]
        [Authorize]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<ActionResult> UploadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "createTime")] DateTime createTime, [FromQuery(Name = "updateTime")] DateTime updateTime, [FromQuery(Name = "fileName")] string? fileName, [FromQuery(Name = "fileSize")] int? fileSize)
        {
            if (Request.Form.Files == null || Request.Form.Files.Count == 0)
            {
                return BadRequest();
            }

            if (!context.Stations.Any(_station => _station.Id == station)) {
                return Forbid();
            }

            if (!await context.TagRecords.AnyAsync(_tag => _tag.Station == station && _tag.Name == tag))
            {
                return Forbid();
            }

            var file = Request.Form.Files[0];
            fileName ??= file.FileName;

            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using (var stream = FileSystem.Create(tempFile))
            {
                await file.CopyToAsync(stream);
            }

            var targetFile = Path.Combine(configuration["Storage:File"] ?? "", station, tag, fileName);
            Directory.CreateDirectory(Path.Combine(configuration["Storage:File"] ?? "", station, tag));

            using FileStream originalFileStream = new(tempFile, FileMode.Open);
            using FileStream decompressedFileStream = FileSystem.Create(targetFile);
            using GZipStream decompressionStream = new(originalFileStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(decompressedFileStream);
            FileSystem.Delete(tempFile);

            FileSystem.SetCreationTimeUtc(targetFile, createTime);
            FileSystem.SetLastWriteTimeUtc(targetFile, updateTime);

            var fileRecord = new FileRecord
            {
                Id = 0,
                UserId = int.Parse(User.FindFirstValue("Id") ?? "0"),
                FileName = fileName,
                FileSize = fileSize ?? (int)file.Length,
                ContentType = file.ContentType,
                Action = FileRecord.FileAction.Upload,
                CreateTime = DateTime.SpecifyKind(createTime, DateTimeKind.Utc),
                UpdateTime = DateTime.SpecifyKind(updateTime, DateTimeKind.Utc),
                SaveTime = DateTime.UtcNow,
                Station = station,
                Tag = tag
            };

            if (await context.FileRecords.AnyAsync(_file => _file.Station == station && _file.Tag == tag && _file.FileName == fileName))
            {
                var _file = await context.FileRecords.FirstAsync(_file => _file.Station == station && _file.Tag == tag && _file.FileName == fileName);
                fileRecord.Id = _file.Id;
                context.FileRecords.Update(fileRecord);
            }
            else
            {
                context.FileRecords.Add(fileRecord);
            }

            await context.SaveChangesAsync();

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
        [Route("get", Name = "GetFile")]
        [Authorize]
        public async Task<ActionResult<FileRecord>> GetFile([FromQuery(Name = "id")] int? id, [FromQuery(Name = "station")] string? station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string? filename)
        {
            var result = await (id == null ? context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename) : context.FileRecords.FirstAsync(item => item.Id == id));
            if (result == null)
            {
                return NotFound();
            }
            var target = Path.Combine(configuration["Storage:File"] ?? "", result.Station, result.Tag, result.FileName);
            if (FileSystem.Exists(target))
            {
                return Ok(result);
            }
            else
            {
                context.FileRecords.Remove(result);
                await context.SaveChangesAsync();
                return NotFound();
            }
        }

        /// <summary>
        /// Get file list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("file/list", Name = "GetFileList")]
        [Authorize]
        public async Task<ActionResult<List<FileRecord>>> GetFileList([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag)
        {
            if (string.IsNullOrEmpty(station))
            {
                return BadRequest();
            }

            if (!await context.TagRecords.AnyAsync(item => item.Station == station && item.Name == tag))
            {
                return NotFound();
            }

            var result = await context.FileRecords.Where(item => item.Station == station && item.Tag == tag).ToListAsync();
            if (result == null)
            {
                return NotFound();
            }

            var list = new List<FileRecord>();
            foreach (var file in result)
            {
                var target = Path.Combine(configuration["Storage:File"] ?? "", file.Station, file.Tag, file.FileName);
                if (FileSystem.Exists(target))
                {
                    list.Add(file);
                }
                else
                {
                    context.FileRecords.Remove(file);
                }
            }
            await context.SaveChangesAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get tag list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        [HttpGet]
        [Route("tags", Name = "GetTagList")]
        [Authorize]
        public async Task<ActionResult<List<TagRecord>>> GetTagList([FromQuery(Name = "station")] string station)
        {
            if (string.IsNullOrEmpty(station))
            {
                return BadRequest();
            }

            var result = await context.TagRecords.Where(item => item.Station == station).ToListAsync();
            return result == null || result.Count == 0 ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("download", Name = "DownloadFile")]
        [Authorize]
        public async Task<ActionResult> DownloadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            var result = await context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename);
            if (result == null)
            {
                return NotFound();
            }

            var targetFile = Path.Combine(configuration["Storage:File"] ?? "", result.Station, result.Tag, result.FileName);

            if (FileSystem.Exists(targetFile))
            {
                var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                using FileStream originalFileStream = new(targetFile, FileMode.Open);
                using FileStream compressedFileStream = FileSystem.Create(tempFile);
                using GZipStream compressionStream = new(compressedFileStream, CompressionMode.Compress);
                originalFileStream.CopyTo(compressionStream);
                var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);

                return File(fs, "application/octet-stream", Path.GetFileName(targetFile));
            }
            else
            {
                context.FileRecords.Remove(result);
                await context.SaveChangesAsync();
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpPost]
        [Route("delete", Name = "DeleteFile")]
        [Authorize]
        public async Task<ActionResult> DeleteFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            var result = await context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename);
            if (result == null)
            {
                return NotFound();
            }
            context.FileRecords.Remove(result);
            context.SaveChanges();

            var targetFile = Path.Combine(configuration["Storage:File"] ?? "", result.Station, result.Tag, result.FileName);

            if (FileSystem.Exists(targetFile))
            {
                FileSystem.Delete(targetFile);
                return Ok();
            }
            else
            {
                context.FileRecords.Remove(result);
                await context.SaveChangesAsync();
                return NotFound();
            }
        }
    }
}