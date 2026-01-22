using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class ExamTimetableModel
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int ExamSubjectId { get; set; }
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RoomNo { get; set; } = string.Empty;
        public int? InvigilatorId { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

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
        public virtual ExamSubjectModel ExamSubject { get; set; }

        [JsonIgnore]
        public virtual StaffModel Invigilator { get; set; }
    }
}


