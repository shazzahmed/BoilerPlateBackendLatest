using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a group of fee types for easier assignment (e.g., "Grade 1 Regular Fees", "Grade 2 Science Stream Fees")
    /// </summary>
    public class FeeGroup : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsSystem { get; set; } = false;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<FeeGroupFeeType> FeeGroupFeeTypes { get; set; } = new List<FeeGroupFeeType>();
    }
}
