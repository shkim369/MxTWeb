using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        public string? ManageCode { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string? Name { get; set; }

        public string? Model { get; set; }

        public string? Manufacturer { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string? Seller { get; set; }

        public string? Location { get; set; }

        public string? Management { get; set; }

        public string? Manager_Main { get; set; }

        public string? Manager_Sub { get; set; }

        public string? Note { get; set; }

    }
}
