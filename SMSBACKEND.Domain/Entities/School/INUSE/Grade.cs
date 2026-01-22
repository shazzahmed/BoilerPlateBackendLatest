using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Grade : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GradeScaleId { get; set; }

        [Required]
        [MaxLength(10)]
        public string GradeName { get; set; } = string.Empty; // A+, A, B+, B, C, D, F

        [Required]
        public decimal MinPercentage { get; set; } = 0;

        [Required]
        public decimal MaxPercentage { get; set; } = 0;

        public decimal GradePoint { get; set; } = 0; // e.g., 4.0, 3.7, 3.3

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = "#000000"; // Hex color for UI

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        [ForeignKey("GradeScaleId")]
        [JsonIgnore]
        public virtual GradeScale GradeScale { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<ExamMarks> ExamMarks { get; set; } = new List<ExamMarks>();

        [JsonIgnore]
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}


