using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Data
{
    public class UpdatePost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "제목을 입력해주세요.")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "내용을 입력해주세요.")]
        public string Content { get; set; } = string.Empty;

        // 첨부파일 정보
        public string? AttachedFileName { get; set; }
        public string? StoredFileName { get; set; }
        public long? FileSize { get; set; }

        // 버전 정보 (예: v1.0.0)
        [MaxLength(50)]
        public string? Version { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
    }
}
