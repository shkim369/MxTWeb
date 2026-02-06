using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Data
{
    public class LoginFailureRecord
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string AttemptedEmail { get; set; } = string.Empty;

        public DateTime AttemptedAt { get; set; }

        [MaxLength(45)] // IPv6 max length
        public string? IpAddress { get; set; }

        [MaxLength(512)]
        public string? UserAgent { get; set; }
    }
}
