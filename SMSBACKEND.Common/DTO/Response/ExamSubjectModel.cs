using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class ExamSubjectModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public decimal MaxMarks { get; set; } = 100;
        public decimal MaxPracticalMarks { get; set; } = 0;
        public decimal PassingMarks { get; set; } = 33;
        public decimal PassingPracticalMarks { get; set; } = 0;
        public DateTime? ExamDate { get; set; }
        public string ExamTime { get; set; } = string.Empty;
        public int Duration { get; set; } = 180;
        public string RoomNo { get; set; } = string.Empty;
        public bool IsOptional { get; set; } = false;
        public string Instructions { get; set; } = string.Empty;

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
        public virtual SubjectModel Subject { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExamMarksModel> ExamMarks { get; set; } = new List<ExamMarksModel>();

        [JsonIgnore]
        public virtual ExamTimetableModel ExamTimetable { get; set; }
    }
}


