using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a type of fee (e.g., Tuition, Transport, Lab, Library)
    /// </summary>
    public class FeeType : BaseEntity
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
        public FeeFrequency FeeFrequency { get; set; } = FeeFrequency.OneTime;

        public bool IsSystem { get; set; } = false;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<FeeGroupFeeType> FeeGroupFeeTypes { get; set; } = new List<FeeGroupFeeType>();
    }
}
