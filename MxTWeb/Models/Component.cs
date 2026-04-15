using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Component
    {
        public int ComponentId { get; set; }
        [Required]
        public string? Product { get; set; }
        [Required]
        public string? Assembly { get; set; }
        [Required]
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? Model { get; set;  }
        public string? Origin { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public string? Category { get; set; }
        public string? Maker { get; set; }
        public string? Material { get; set; }
        public string? Color_Etch { get; set; }
        public string? Seller { get; set; }
        public string? LeadTime { get; set; }
        public int Stock { get; set; }
        public string? Note { get; set; }
        public ICollection<CompHistory> CompHistories { get; set; } = new List<CompHistory>();
        public ICollection<BomDetail> BomDetails { get; set; } = new List<BomDetail>();
    }
}
