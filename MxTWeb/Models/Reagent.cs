using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Reagent
    {
        public int Id { get; set; }

        [Required]
        public string? Code { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        public string? Manufacturer { get; set; }

        public string? Amount { get; set; }

        public string? Cat { get; set; }

        public DateOnly ReceiptDate { get; set; }

        public int Quantity { get; set; }

        public int Lot { get; set; }

        public string? Location { get; set; }
    }
}
