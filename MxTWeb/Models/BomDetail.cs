using MxTWeb.Components.Pages.ComponentPages;
using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class BomDetail
    {
        public int BomDetailId { get; set; } // PK
        public int BomId { get; set; }  // FK 
        public int ComponentId { get; set; }  // FK 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public Bom Bom { get; set; } = null!; // Navigation property, PK
        public Component Component { get; set; } = null!; // Navigation property, PK
    }
}
