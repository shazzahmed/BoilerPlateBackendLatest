
using System.ComponentModel.DataAnnotations.Schema;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeGroupFeeTypeModel : BaseClass
    {
        public int Id { get; set; }
        // Required foreign keys
        public int FeeGroupId { get; set; }
        public int FeeTypeId { get; set; }
        public int? FeeDiscountId { get; set; }
        // Fee metadata
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }

        public string? FineType { get; set; }
        public decimal FinePercentage { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsSystem { get; set; } = false;
        // Navigation properties
        [ForeignKey("FeeGroupId")]
        public virtual FeeGroupModel? FeeGroup { get; set; }

        [ForeignKey("FeeTypeId")]
        public virtual FeeTypeModel? FeeType { get; set; }

        [ForeignKey("FeeDiscountId")]
        public FeeDiscountModel? FeeDiscount { get; set; }
    }
}
