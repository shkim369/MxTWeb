using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MxTWeb.Models
{
    public class ChipPressure
    {
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public string? Lot { get; set; }

        [Required]
        public DateTime? TestDate { get; set; }

        public DateTime? ProduceDate { get; set; }

        public int TestNum { get; set; }

        public string? WaferInfo { get; set; }

        public int Pressure { get; set; }

        public string? passFail { get; set; }

        public string? Note { get; set; }
    }
}
