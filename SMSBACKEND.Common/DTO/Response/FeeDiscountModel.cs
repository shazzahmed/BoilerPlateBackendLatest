
using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeDiscountModel : BaseClass
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(100)]
        public required string Code { get; set; }
        public string? Description { get; set; }

        public DiscountType DiscountType { get; set; }
        public int NoOfUseCount { get; set; }
        public float? DiscountPercentage { get; set; } = 0.0f;
        public float? DiscountAmount { get; set; } = 0.0f;
        public DateTime? ExpiryDate { get; set; }
        public bool IsRecurring { get; set; } // Monthly or One-Time
        public bool IsSystem { get; set; } = false;
    }
}
