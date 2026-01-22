namespace Common.DTO.Response
{
    /// <summary>
    /// Fee preview model for applications
    /// </summary>
    public class FeePreviewResponse
    {
        public int? FeeGroupFeeTypeId { get; set; }
        public string FeeGroupName { get; set; }
        public List<FeeTypePreview> FeeTypes { get; set; } = new List<FeeTypePreview>();
        public decimal TotalAnnualFees { get; set; }
        public decimal TotalMonthlyFees { get; set; }
        public decimal TotalOneTimeFees { get; set; }
    }

    public class FeeTypePreview
    {
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public string FeeTypeCode { get; set; }
        public string FeeFrequency { get; set; }  // "OneTime", "Monthly", "Annually", etc.
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string FineType { get; set; }
        public decimal FineAmount { get; set; }
        public decimal FinePercentage { get; set; }
        public string DiscountName { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}


