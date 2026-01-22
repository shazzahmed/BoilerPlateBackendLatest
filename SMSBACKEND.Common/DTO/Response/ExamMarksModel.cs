using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class ExamMarksModel
    {
        public int Id { get; set; }
        public int ExamSubjectId { get; set; }
        public int StudentId { get; set; }
        public decimal TheoryMarks { get; set; } = 0;
        public decimal PracticalMarks { get; set; } = 0;
        public decimal TotalMarks => TheoryMarks + PracticalMarks;
        public bool IsAbsent { get; set; } = false;
        public int? GradeId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string EnteredBy { get; set; } = string.Empty;
        public DateTime? EnteredDate { get; set; }
        public string VerifiedBy { get; set; } = string.Empty;
        public DateTime? VerifiedDate { get; set; }
        public MarksStatus Status { get; set; } = MarksStatus.Draft;

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
        public virtual ExamSubjectModel ExamSubject { get; set; }

        [JsonIgnore]
        public virtual StudentModel Student { get; set; }

        [JsonIgnore]
        public virtual GradeModel Grade { get; set; }
    }
}


