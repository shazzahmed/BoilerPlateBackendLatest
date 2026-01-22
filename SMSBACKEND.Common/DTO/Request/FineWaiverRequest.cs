using System;
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class FineWaiverRequest
    {
        public int FeeAssignmentId { get; set; }
        public int StudentId { get; set; }
        public decimal FineAmount { get; set; }
        public decimal WaiverAmount { get; set; }
        public string Reason { get; set; }
    }

    public class FineWaiverApprovalRequest
    {
        public int Id { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovalNote { get; set; }
    }
}

