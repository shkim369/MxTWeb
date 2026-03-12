using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class CompHistory
    {
        public int CompHistoryId { get; set; }
        public int ComponentId { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public string? state { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Account { get; set; }
        public DateTime? EntryDate { get; set; }
        public Component Component { get; set; } = null!; // Navigation property
    }
}
