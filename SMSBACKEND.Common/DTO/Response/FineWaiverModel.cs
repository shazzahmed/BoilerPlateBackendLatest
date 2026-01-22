using System;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FineWaiverModel : BaseClass
    {
        public int Id { get; set; }
        public int FeeAssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? FeeTypeName { get; set; }
        public string? FeeGroupName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        
        public decimal OriginalFineAmount { get; set; }
        public decimal WaiverAmount { get; set; }
        public decimal RemainingFineAmount { get; set; }
        
        public string? Reason { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected
                     
        public string? RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalNote { get; set; }
    }
}
