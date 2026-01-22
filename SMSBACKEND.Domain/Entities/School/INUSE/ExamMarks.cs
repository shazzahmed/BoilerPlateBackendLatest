using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class ExamMarks : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public decimal TheoryMarks { get; set; } = 0;

        public decimal PracticalMarks { get; set; } = 0;

        [NotMapped]
        public decimal TotalMarks => TheoryMarks + PracticalMarks;

        public bool IsAbsent { get; set; } = false;

        public int? GradeId { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EnteredBy { get; set; } = string.Empty;

        public DateTime? EnteredDate { get; set; }

        [MaxLength(100)]
        public string VerifiedBy { get; set; } = string.Empty;

        public DateTime? VerifiedDate { get; set; }

        [Required]
        public MarksStatus Status { get; set; } = MarksStatus.Draft;

        // Navigation properties
        [ForeignKey("ExamSubjectId")]
        [JsonIgnore]
        public virtual ExamSubject ExamSubject { get; set; }

        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; }

        [ForeignKey("GradeId")]
        [JsonIgnore]
        public virtual Grade Grade { get; set; }
    }
}


