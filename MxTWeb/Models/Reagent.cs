using System.ComponentModel.DataAnnotations;

namespace MxTWeb.Models
{
    public class Reagent
    {
        public int Id { get; set; }

        [Required]
        public string? Code { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        public string? Manufacturer { get; set; }

        public string? Amount { get; set; }

        public string? Cat { get; set; }
    }
}
