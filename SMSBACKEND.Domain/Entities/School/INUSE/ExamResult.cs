using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class ExamResult : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public decimal TotalMarksObtained { get; set; } = 0;

        public decimal TotalMaxMarks { get; set; } = 0;

        [NotMapped]
        public decimal Percentage => TotalMaxMarks > 0 ? (TotalMarksObtained / TotalMaxMarks) * 100 : 0;

        public int? OverallGradeId { get; set; }

        public decimal GPA { get; set; } = 0;

        public int? Rank { get; set; } // Class rank

        public int? SectionRank { get; set; }

        [Required]
        [MaxLength(20)]
        public string Result { get; set; } = "Pending"; // Pass, Fail, Absent, Pending

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public DateTime? GeneratedDate { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedDate { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        [JsonIgnore]
        public virtual Exam Exam { get; set; }

        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; }

        [ForeignKey("OverallGradeId")]
        [JsonIgnore]
        public virtual Grade OverallGrade { get; set; }
    }
}


