using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MxTWeb.Services;

namespace MxTWeb.Services
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;

        //public FileUploadController(IWebHostEnvironment environment)
        public FileUploadController(IConfiguration configuration)
        {
            //_environment = environment;
            //_uploadPath = Path.Combine(_environment.WebRootPath, "uploads");

            // Docker 환경에서는 /app/uploads 사용
            _uploadPath = configuration["UploadPath"] ?? "/app/uploads";


            // uploads 폴더가 없으면 생성
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("파일이 선택되지 않았습니다.");

                // 파일 크기 제한 (10MB)
                if (file.Length > 10 * 1024 * 1024)
                    return BadRequest("파일 크기는 10MB를 초과할 수 없습니다.");

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
                    UploadDate = DateTime.Now
                };

                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"파일 업로드 실패: {ex.Message}");
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
                return StatusCode(500, $"파일 목록 조회 실패: {ex.Message}");
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
                return StatusCode(500, $"파일 다운로드 실패: {ex.Message}");
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
                return StatusCode(500, $"파일 삭제 실패: {ex.Message}");
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
}