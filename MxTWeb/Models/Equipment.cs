using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        public int ManageCode { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string? Name { get; set; }

        public string? Model { get; set; }

        public string? Manufacturer { get; set; }

        public DateOnly PurchaseDate { get; set; }

        public string? Seller { get; set; }

        public string? Location { get; set; }

        public string? Management { get; set; }

        public string? Manager { get; set; }

        public string? Note { get; set; }

    }
}
