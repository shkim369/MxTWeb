using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class ChipUsage
    {
        public int Id { get; set; }

        [Required]
        public string? Chip { get; set; }

        [Required]
        public string? Division { get; set; }

        [Required]
        public DateTime? UseDate { get; set; }

        public string? Producer { get; set; }

        public string? Wafer { get; set; }

        public string? WaferInfo { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public string? Operator { get; set; }

        public int Available { get; set; }

        public int Production { get; set; }

        public int Left { get; set; }

        public string? Lot { get; set; }

        public string? Note { get; set; }

    }
    public enum ChipType
    {
        Large,
        Small
    }
    public enum ProductStatus
    {
        Delivery,
        Production,
        Exhausted
    }
    public enum WaferType
    {
        SOI,
        SOG
    }
}
