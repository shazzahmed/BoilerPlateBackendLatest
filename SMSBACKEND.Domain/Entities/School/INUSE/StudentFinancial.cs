using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class StudentFinancial : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public float? AdmissionFees { get; set; }

        public float? SecurityDeposit { get; set; }

        [Required]
        public decimal MonthlyFees { get; set; }

        [Required]
        public decimal Arrears { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;
    }
}