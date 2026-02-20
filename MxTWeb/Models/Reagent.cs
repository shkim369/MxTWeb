using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Reagent
    {
        public int Id { get; set; }

        [Required]
        public string? Code { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Manufacturer { get; set; }

        public string? Amount { get; set; }

        public string? Cat { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public int Quantity { get; set; }

        public string? Lot { get; set; }

        public string? Location { get; set; }

        public string? Comment { get; set; }    
    }
}
