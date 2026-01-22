using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    public class Exam : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ExamName { get; set; } = string.Empty;

        [Required]
        public ExamType ExamType { get; set; }

        [Required]
        public int ClassId { get; set; }

        public int? SectionId { get; set; } // Nullable for class-wide exams

        [Required]
        [MaxLength(10)]
        public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-25"

        [Required]
        public Term Term { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal TotalMarks { get; set; } = 0;

        public decimal PassingMarks { get; set; } = 0;

        [Required]
        public ExamStatus Status { get; set; } = ExamStatus.Draft;

        public bool IsPublished { get; set; } = false;

        public DateTime? PublishedDate { get; set; }

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; }

        [ForeignKey("SectionId")]
        [JsonIgnore]
        public virtual Section Section { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExamSubject> ExamSubjects { get; set; } = new List<ExamSubject>();

        [JsonIgnore]
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

        [JsonIgnore]
        public virtual ICollection<ExamTimetable> ExamTimetables { get; set; } = new List<ExamTimetable>();
    }
}


