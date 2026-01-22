using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a category/head for expense transactions
    /// </summary>
    public class ExpenseHead : BaseEntity
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
        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
