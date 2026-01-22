using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an income transaction for the school
    /// </summary>
    public class Income : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IncomeHeadId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? InvoiceNo { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime Date { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? AttachDocument { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Navigation properties
        [ForeignKey("IncomeHeadId")]
        [JsonIgnore]
        public virtual IncomeHead IncomeHead { get; set; } = null!;
    }
}
