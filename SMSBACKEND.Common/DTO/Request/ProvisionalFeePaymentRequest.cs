using System;
using System.Collections.Generic;

namespace Common.DTO.Request
{
    /// <summary>
    /// Request model for provisional fee payment (PreAdmission/Application fees)
    /// </summary>
    public class ProvisionalFeePaymentRequest
    {
        public int ApplicationId { get; set; } // PreAdmission ID
        public List<FeePaymentItem> FeePayments { get; set; } = new List<FeePaymentItem>();
        public DateTime PaymentDate { get; set; }
        public string CollectedBy { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        
        // Notification preferences
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
    }
}

