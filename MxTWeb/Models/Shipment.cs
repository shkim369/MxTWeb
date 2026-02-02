using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }
        [Required]
        public string? Division { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Destination { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        public string? SerialNumber { get; set; }
        public string? LotNumber { get; set; }
        public string? Chip { get; set; }
        public int Quantity { get; set; }
        public string? SWVersion { get; set; }
        public string? HWVersion { get; set; }
        public string? Comments { get; set; }
    }
}
