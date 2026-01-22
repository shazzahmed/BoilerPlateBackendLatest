using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a fee discount that can be applied to students
    /// </summary>
    public class FeeDiscount : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        public int NoOfUseCount { get; set; } = 0;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "DateTime2")]
        public DateTime? ExpiryDate { get; set; }

        public bool IsRecurring { get; set; } = false;  // Monthly or One-Time

        public bool IsSystem { get; set; } = false;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<FeeGroupFeeType> FeeGroupFeeTypes { get; set; } = new List<FeeGroupFeeType>();

        [JsonIgnore]
        public virtual ICollection<FeeAssignment> FeeAssignments { get; set; } = new List<FeeAssignment>();
    }
}
