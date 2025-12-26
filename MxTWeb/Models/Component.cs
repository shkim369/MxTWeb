using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Component
    {
        public int Id { get; set; }

        [Required]
        public string? Assembly { get; set; }

        [Required]
        public string? PartNumber { get; set; }

        public string? Description { get; set; }

        public string? Model { get; set;  }

        public string? Origin { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }

        public int TotalPrice { get; set; }

        public string? Category { get; set; }

        public string? Maker { get; set; }

        public string? Material { get; set; }
        
        public string? Color_Etch { get; set; }

        public string? Seller { get; set; }

        public string? LeadTime { get; set; }

        public int Stock { get; set; }

        public int Required { get; set; }

        public string? Note { get; set; }
    }

    public class BOM
    {
        public int BomId { get; set; }  // PK
        public string ProductName { get; set; }
        public string BomVersion { get; set; }
        public List<BOMDetail> Details { get; set; }
    }

    public class BOMDetail
    {
        public int BomDetailId { get; set; }
        public int BomId { get; set; }  // FK 
        public int PartId { get; set; }  // FK 
        public decimal Quantity { get; set; }
        public Component component { get; set; } // Navigation property
    }
}
