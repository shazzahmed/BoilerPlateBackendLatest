using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a category/head for income transactions
    /// </summary>
    public class IncomeHead : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
    }
}
