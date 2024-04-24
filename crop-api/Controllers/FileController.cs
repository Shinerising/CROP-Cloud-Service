using CROP.API.Data;
using CROP.API.Models;
using CROP.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
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
        private readonly string _root = configuration[Env.StorageFolder] ?? configuration["Storage:File"] ?? "/storage";

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
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "createTime")] DateTime createTime, [FromQuery(Name = "updateTime")] DateTime updateTime, [FromQuery(Name = "fileName")] string? fileName, [FromQuery(Name = "fileSize")] int? fileSize)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

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

            var targetFile = Path.Combine(_root, station, tag, fileName);
            Directory.CreateDirectory(Path.Combine(_root, station, tag));

            {
                using FileStream originalFileStream = new(tempFile, FileMode.Open);
                using FileStream decompressedFileStream = FileSystem.Create(targetFile);
                using GZipStream decompressionStream = new(originalFileStream, CompressionMode.Decompress);
                await decompressionStream.CopyToAsync(decompressedFileStream);
                FileSystem.Delete(tempFile);
            }

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
        [Route("real/preview", Name = "GetRealFilePreview")]
        [Authorize]
        public async Task<ActionResult<RealFilePreview>> GetRealFilePreview([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename, [FromQuery(Name = "charset")] string? charset)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var path = Path.Combine(_root, station, tag, filename);

            if (!FileSystem.Exists(path))
            {
                return NotFound();
            }

            return await Task.Run(() =>
            {
                var file = new FileInfo(path);
                string content = "";
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.GetEncoding(charset ?? "utf-8"), true);
                char[] bytes = new char[102400];
                int count = sr.ReadBlock(bytes, 0, 102400);
                content = new string(bytes, 0, count);
                var record = new RealFilePreview(file.Name, file.Extension, station, tag, file.Length, file.CreationTime, file.LastWriteTime, content);
                return Ok(record);
            });
        }
        /// <summary>
        /// Get file information.
        /// </summary>
        /// <param name="id">The id of the file.</param>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        /// <param name="filename">The name of the file.</param>
        [HttpGet]
        [Route("real/get", Name = "GetRealFile")]
        [Authorize]
        public async Task<ActionResult<RealFile>> GetRealFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var path = Path.Combine(_root, station, tag, filename);

            if (!FileSystem.Exists(path))
            {
                return NotFound();
            }

            return await Task.Run(() =>
            {
                var file = new FileInfo(path);
                var record = new RealFile(file.Name, file.Extension, station, tag, file.Length, file.CreationTime, file.LastWriteTime);
                return Ok(record);
            });
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
        public async Task<ActionResult<FileRecord>> GetFile([FromQuery(Name = "id")] int? id, [FromQuery(Name = "station")] string? station, [FromQuery(Name = "tag")] string? tag, [FromQuery(Name = "filename")] string? filename)
        {
            tag = tag?.ToLower();

            var result = await (id == null ? context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename) : context.FileRecords.FirstAsync(item => item.Id == id));
            if (result == null)
            {
                return NotFound();
            }
            var target = Path.Combine(_root, result.Station, result.Tag, result.FileName);
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
        /// <param name="ext">The extension of the file.</param>
        [HttpGet]
        [Route("real/list", Name = "GetRealFileList")]
        [Authorize]
        public async Task<ActionResult<List<RealFile>>> GetRealFileList([FromQuery(Name = "start")] int start, [FromQuery(Name = "end")] int end, [FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "ext")] string ext)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var startTime = DateTimeOffset.FromUnixTimeSeconds(start);
            var endTime = DateTimeOffset.FromUnixTimeSeconds(end);

            var folder = Path.Combine(_root, station, tag);

            if (!Directory.Exists(folder))
            {
                return NotFound();
            }

            var pattern = string.IsNullOrEmpty(ext) ? "*.*" : $"*.{ext}";

            return await Task.Run(() =>
            {
                var list = Directory.EnumerateFiles(folder, pattern)
                .Select(file => new FileInfo(file))
                .OrderBy(info => info.CreationTimeUtc)
                .Where(info => info.CreationTimeUtc >= startTime && info.CreationTimeUtc <= endTime)
                .Take(1024)
                .Select(info => new RealFile(info.Name, info.Extension, station, tag, info.Length, info.CreationTime, info.LastWriteTime)).ToList();
                return Ok(list);
            }); 
        }

        /// <summary>
        /// Get file list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("list", Name = "GetFileList")]
        [Authorize]
        public async Task<ActionResult<List<FileRecord>>> GetFileList([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

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
                var target = Path.Combine(_root, file.Station, file.Tag, file.FileName);
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
        /// Get file list.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("list/filter", Name = "GetFileListFilter")]
        [Authorize]
        public async Task<ActionResult<List<FileRecord>>> GetFileListFilter([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "start")] DateTimeOffset startTime, [FromQuery(Name = "end")] DateTimeOffset endTime)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            if (!await context.TagRecords.AnyAsync(item => item.Station == station && item.Name == tag))
            {
                return NotFound();
            }

            var result = await context.FileRecords.Where(item => item.Station == station && item.Tag == tag && item.CreateTime >= startTime && item.CreateTime <= endTime).ToListAsync();
            if (result == null)
            {
                return NotFound();
            }

            var list = new List<FileRecord>();
            foreach (var file in result)
            {
                var target = Path.Combine(_root, file.Station, file.Tag, file.FileName);
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

        [HttpGet("shift", Name = "GetShiftInfo")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ShiftData>> GetShiftInfo([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var path = Path.Combine(_root, station, tag, filename);
            var result = await context.ShiftDatas.FirstOrDefaultAsync(item => item.FileName == path);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="station">The station to download the file for.</param>
        /// <param name="filename">The name of the file.</param>
        /// <param name="tag">The tag of the file.</param>
        [HttpGet]
        [Route("real/download", Name = "DownloadRealFile")]
        [Authorize]
        public async Task<ActionResult> DownloadRealFile([FromQuery(Name = "station")] string station, [FromQuery(Name = "tag")] string tag, [FromQuery(Name = "filename")] string filename)
        {
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var targetFile = Path.Combine(_root, station, tag, filename);

            if (FileSystem.Exists(targetFile))
            {
                await Task.Delay(10);
                FileStream fs = new(targetFile, FileMode.Open, FileAccess.Read, FileShare.None, 4096);
                return File(fs, "application/octet-stream", Path.GetFileName(targetFile));
            }
            else
            {
                return NotFound();
            }
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
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var result = await context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename);
            if (result == null)
            {
                return NotFound();
            }

            var targetFile = Path.Combine(_root, result.Station, result.Tag, result.FileName);

            if (FileSystem.Exists(targetFile))
            {
                var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                {
                    using FileStream originalFileStream = new(targetFile, FileMode.Open);
                    using FileStream compressedFileStream = FileSystem.Create(tempFile);
                    using GZipStream compressionStream = new(compressedFileStream, CompressionMode.Compress);
                    await originalFileStream.CopyToAsync(compressionStream);
                }

                FileStream fs = new(tempFile, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
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
            if (string.IsNullOrEmpty(station) || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(filename))
            {
                return BadRequest();
            }
            tag = tag.ToLower();

            var result = await context.FileRecords.FirstAsync(item => item.Station == station && item.Tag == tag && item.FileName == filename);
            if (result == null)
            {
                return NotFound();
            }
            context.FileRecords.Remove(result);
            context.SaveChanges();

            var targetFile = Path.Combine(_root, result.Station, result.Tag, result.FileName);

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