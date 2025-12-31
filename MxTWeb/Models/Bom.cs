using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Bom
    {
        public int BomId { get; set; }  // PK
        public string? ProductName { get; set; }
        public string? BomVersion { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<BomDetail> BomDetails { get; set; } = new List<BomDetail>();
    }
}
