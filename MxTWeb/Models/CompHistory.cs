using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class CompHistory
    {
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        public string? state { get; set; }

        [Required]
        public string? PartNumber { get; set; }

        public int Quantity { get; set; }

        public string? Account { get; set; }

        public DateTime? EntryDate { get; set; }

        public Component? Component { get; set; }
    }
}
