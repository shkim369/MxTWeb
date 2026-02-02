using DocumentFormat.OpenXml.Presentation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MxTWeb.Models
{
    public class Cyropreserved
    {
        public int Id { get; set; }
        //public string? SerialNumber { get; set; }
        public string? Code { get; set; }
        [Required]
        public string? Division { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public DateTime? ReceiptDate { get; set; }
        public string? Recipient { get; set; }
        public string? Supplier { get; set; }
        public DateTime? FreezeDate { get; set; }
        public int Quantity { get; set; }
        public string? Lot { get; set; }
        public string? Density { get; set; }
        public string? Location1 { get; set; }
        public string? Location2 { get; set; }
        public string? Test { get; set; }
        public string? Parent { get; set; }
        public int level { get; set; }
        public string? Origin { get; set; }
        public string? Note { get; set; }
    }
}
