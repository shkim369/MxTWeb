using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MxTWeb.Models
{
    public class Cyropreserved
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        public DateTime? ReceiptDate { get; set; }

        [Required]
        public string? Recipient { get; set; }

        [Required]
        public string? Supplier { get; set; }

        public DateTime? FreezeDate { get; set; }

        [Range(0, 100)]
        public string? Quantity { get; set; }

        public string? Lot { get; set; }

        public string? Density { get; set; }

        public string? Location { get; set; }

        public string? Note { get; set; }

    }
}
