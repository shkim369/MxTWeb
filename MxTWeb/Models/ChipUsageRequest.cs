using System.ComponentModel.DataAnnotations;
using MxTWeb.Enums;

namespace MxTWeb.Models
{
    public class ChipusageRequest
    {
        public int Id { get; set; }

        [Required]
        public int ChipType { get; set; }

        [Required]
        public string? Applicant { get; set; }  // 신청자

        [Required]
        public DateTime? ApplicationDate { get; set; }  // 신청일

        [Required]
        public int RequestedQuantity { get; set; }  // 신청수량

        public string? Purpose { get; set; }  // 사용목적

        public RequestStatusEnum Status { get; set; } = RequestStatusEnum.Pending;  // 승인상태

        public string? Approver { get; set; }  // 승인자

        public DateTime? ProcessedDate { get; set; }  // 처리일시

        public string? RejectionReason { get; set; }  // 반려사유

        public string? Note { get; set; }  // 비고

        public DateTime CreatedDate { get; set; } = DateTime.Now;  // 생성일
    }
}
