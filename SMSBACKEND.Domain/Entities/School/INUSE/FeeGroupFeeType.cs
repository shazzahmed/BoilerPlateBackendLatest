using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Fee Groups and Fee Types with additional metadata
    /// </summary>
    public class FeeGroupFeeType : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Required foreign keys
        [Required]
        public int FeeGroupId { get; set; }

        [Required]
        public int FeeTypeId { get; set; }

        public int? FeeDiscountId { get; set; }

        // Fee metadata
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? DueDate { get; set; }

        [Required]
        public FinePolicyType FineType { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal FinePercentage { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FineAmount { get; set; } = 0;

        public bool IsSystem { get; set; } = false;

        // Navigation properties
        [ForeignKey("FeeGroupId")]
        [JsonIgnore]
        public virtual FeeGroup FeeGroup { get; set; } = null!;

        [ForeignKey("FeeTypeId")]
        [JsonIgnore]
        public virtual FeeType FeeType { get; set; } = null!;

        [ForeignKey("FeeDiscountId")]
        [JsonIgnore]
        public virtual FeeDiscount? FeeDiscount { get; set; }

        [JsonIgnore]
        public virtual ICollection<FeeAssignment> FeeAssignments { get; set; } = new List<FeeAssignment>();
    }
}
