using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class ExamResultModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public decimal TotalMarksObtained { get; set; } = 0;
        public decimal TotalMaxMarks { get; set; } = 0;
        public decimal Percentage => TotalMaxMarks > 0 ? (TotalMarksObtained / TotalMaxMarks) * 100 : 0;
        public int? OverallGradeId { get; set; }
        public decimal GPA { get; set; } = 0;
        public int? Rank { get; set; }
        public int? SectionRank { get; set; }
        public string Result { get; set; } = "Pending";
        public string Remarks { get; set; } = string.Empty;
        public DateTime? GeneratedDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedDate { get; set; }

        // BaseEntity properties
        public int TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ExamModel Exam { get; set; }

        [JsonIgnore]
        public virtual StudentModel Student { get; set; }

        [JsonIgnore]
        public virtual GradeModel OverallGrade { get; set; }
    }
}


