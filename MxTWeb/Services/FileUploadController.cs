using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MxTWeb.Data;

namespace MxTWeb.Services
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKeyAuth]
    public class FileUploadController : ControllerBase
    {
        private readonly string _uploadPath;        // 일반 파일 (사진, 엑셀, PDF 등)
        private readonly string _updateUploadPath;  // 업데이트 파일 전용
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(IConfiguration configuration, IDbContextFactory<ApplicationDbContext> dbFactory, ILogger<FileUploadController> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
            _uploadPath = configuration["UploadPath"] ?? "/docker/web/web-uploads";
            _updateUploadPath = configuration["UpdateUploadPath"] ?? "/docker/web/web-uploads/update";

            _logger.LogInformation("UploadPath: {UploadPath}, UpdateUploadPath: {UpdateUploadPath}", _uploadPath, _updateUploadPath);

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
            if (!Directory.Exists(_updateUploadPath))
            {
                try
                {
                    Directory.CreateDirectory(_updateUploadPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UpdateUploadPath 폴더 생성 실패: {Path}", _updateUploadPath);
                }
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("파일이 선택되지 않았습니다.");

                // 파일 크기 제한 (50MB)
                if (file.Length > 50 * 1024 * 1024)
                    return BadRequest("파일 크기는 50MB를 초과할 수 없습니다.");

                // 고유한 파일명 생성
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_uploadPath, uniqueFileName);

                // 파일 저장
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileInfo = new FileInfoDto
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = file.FileName,
                    StoredFileName = uniqueFileName,
                    Size = file.Length,
                    ContentType = file.ContentType,
                    UploadDate = DateTime.UtcNow
                };

                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 업로드 실패. FileName={FileName}", file?.FileName);
                return StatusCode(500, "파일 업로드에 실패했습니다.");
            }
        }

        [HttpGet("list")]
        public IActionResult GetFileList()
        {
            try
            {
                var files = Directory.GetFiles(_uploadPath)
                    .Select(filePath =>
                    {
                        var fileInfo = new FileInfo(filePath);
                        var storedFileName = Path.GetFileName(filePath);
                        var originalFileName = storedFileName.Substring(37); // GUID(36) + _(1) 제거

                        return new FileInfoDto
                        {
                            Id = Path.GetFileNameWithoutExtension(filePath).Substring(0, 36),
                            FileName = originalFileName,
                            StoredFileName = storedFileName,
                            Size = fileInfo.Length,
                            UploadDate = fileInfo.CreationTime
                        };
                    })
                    .OrderByDescending(f => f.UploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 목록 조회 실패");
                return StatusCode(500, "파일 목록 조회에 실패했습니다.");
            }
        }

        [HttpGet("download/{storedFileName}")]
        public IActionResult DownloadFile(string storedFileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, storedFileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("파일을 찾을 수 없습니다.");

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                var originalFileName = storedFileName.Substring(37);
                var contentType = GetContentType(filePath);

                return File(memory, contentType, originalFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 다운로드 실패. StoredFileName={StoredFileName}", storedFileName);
                return StatusCode(500, "파일 다운로드에 실패했습니다.");
            }
        }

        [HttpDelete("delete/{storedFileName}")]
        public IActionResult DeleteFile(string storedFileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, storedFileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("파일을 찾을 수 없습니다.");

                System.IO.File.Delete(filePath);
                return Ok("파일이 삭제되었습니다.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 삭제 실패. StoredFileName={StoredFileName}", storedFileName);
                return StatusCode(500, "파일 삭제에 실패했습니다.");
            }
        }

        // ========== 업데이트 파일 전용 엔드포인트 ==========

        [HttpPost("update/upload")]
        [AllowAnonymous]  // Blazor Server HttpClient는 쿠키를 전달하지 않으므로
        [RequestSizeLimit(52428800)] // 50MB
        public async Task<IActionResult> UploadUpdateFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("파일이 선택되지 않았습니다.");
                }

                if (file.Length > 50 * 1024 * 1024)
                {
                    return BadRequest("파일 크기는 50MB를 초과할 수 없습니다.");
                }

                if (!Directory.Exists(_updateUploadPath))
                {
                    Directory.CreateDirectory(_updateUploadPath);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(_updateUploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("업데이트 파일 저장 성공. Path={FilePath}, Size={Size}", filePath, file.Length);

                var fileInfo = new UpdateFileInfoDto
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = file.FileName,
                    StoredFileName = uniqueFileName,
                    Size = file.Length,
                    ContentType = string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    UploadDate = DateTime.UtcNow,
                };

                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업데이트 파일 업로드 실패. FileName={FileName}", file?.FileName);
                return StatusCode(500, "파일 업로드에 실패했습니다.");
            }
        }


        // 장비 업데이트 목록 확인 (내부용으로 사용)
        [HttpGet("update/list/{equipment}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpdateFileList(string equipment)
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();
                var posts = await context.UpdatePosts
                    .Where(p => p.Equipment == equipment && p.StoredFileName != null && p.StoredFileName != "")
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new UpdateFileInfoDto
                    {
                        Id = p.Id.ToString(),
                        FileName = p.AttachedFileName ?? "",
                        StoredFileName = p.StoredFileName ?? "",
                        Size = p.FileSize ?? 0,
                        ContentType = "application/octet-stream",
                        UploadDate = p.CreatedAt,
                        Version = p.Version ?? "",
                        IsLatest = p.IsLatest,
                    })
                    .ToListAsync();

                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업데이트 파일 목록 조회 실패. Equipment={Equipment}", equipment);
                return StatusCode(500, "파일 목록 조회에 실패했습니다.");
            }
        }

        /// 최신 버전 파일 정보 조회 (판매용 장비에서 사용)
        [HttpGet("update/latest/{equipment}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLatestUpdateFile(string equipment)
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();
                var latestPost = await context.UpdatePosts
                    .Where(p => p.IsLatest && p.Equipment == equipment && p.StoredFileName != null && p.StoredFileName != "")
                    .FirstOrDefaultAsync();

                if (latestPost == null)
                {
                    return NotFound($"'{equipment}'의 최신 업데이트가 없습니다.");
                }

                var result = new UpdateFileInfoDto
                {
                    Id = latestPost.Id.ToString(),
                    FileName = latestPost.AttachedFileName ?? "",
                    StoredFileName = latestPost.StoredFileName ?? "",
                    Size = latestPost.FileSize ?? 0,
                    ContentType = "application/octet-stream",
                    UploadDate = latestPost.CreatedAt,
                    Version = latestPost.Version ?? "",
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "최신 버전 조회 실패. Equipment={Equipment}", equipment);
                return StatusCode(500, "최신 버전 조회에 실패했습니다.");
            }
        }

        /// 최신 버전 파일 직접 다운로드 (판매용 장비에서 사용)
        [HttpGet("update/latest/{equipment}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadLatestUpdateFile(string equipment)
        {
            try
            {
                using var context = await _dbFactory.CreateDbContextAsync();
                var latestPost = await context.UpdatePosts
                    .Where(p => p.IsLatest && p.Equipment == equipment && p.StoredFileName != null && p.StoredFileName != "")
                    .FirstOrDefaultAsync();

                if (latestPost == null || string.IsNullOrEmpty(latestPost.StoredFileName))
                {
                    return NotFound($"'{equipment}'의 최신 업데이트가 없습니다.");
                }

                var filePath = Path.Combine(_updateUploadPath, latestPost.StoredFileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("파일을 찾을 수 없습니다.");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var contentType = GetContentType(filePath);
                return File(memory, contentType, latestPost.AttachedFileName ?? latestPost.StoredFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "최신 버전 다운로드 실패. Equipment={Equipment}", equipment);
                return StatusCode(500, "최신 버전 다운로드에 실패했습니다.");
            }
        }

        // 다운로드 - 파일 이름으로 확인
        [HttpGet("update/download/{storedFileName}")]
        [AllowAnonymous]
        public IActionResult DownloadUpdateFile(string storedFileName)
        {
            try
            {
                var filePath = Path.Combine(_updateUploadPath, storedFileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("파일을 찾을 수 없습니다.");

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                var originalFileName = storedFileName.Length > 37 ? storedFileName.Substring(37) : storedFileName;
                var contentType = GetContentType(filePath);

                return File(memory, contentType, originalFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업데이트 파일 다운로드 실패. StoredFileName={StoredFileName}", storedFileName);
                return StatusCode(500, "파일 다운로드에 실패했습니다.");
            }
        }

        [HttpDelete("update/delete/{storedFileName}")]
        [AllowAnonymous]
        public IActionResult DeleteUpdateFile(string storedFileName)
        {
            try
            {
                var filePath = Path.Combine(_updateUploadPath, storedFileName);

                if (!System.IO.File.Exists(filePath))
                    return NotFound("파일을 찾을 수 없습니다.");

                System.IO.File.Delete(filePath);
                return Ok("파일이 삭제되었습니다.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업데이트 파일 삭제 실패. StoredFileName={StoredFileName}", storedFileName);
                return StatusCode(500, "파일 삭제에 실패했습니다.");
            }
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

    }

    public class FileInfoDto
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string StoredFileName { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
    }

    public class UpdateFileInfoDto : FileInfoDto
    {
        public string Version { get; set; } = "";
        public bool IsLatest { get; set; } = false;
        public string Equipment { get; set; } = "";
    }
}