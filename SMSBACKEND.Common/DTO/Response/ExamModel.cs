using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class ExamModel
    {
        public int Id { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public int ClassId { get; set; }
        public int? SectionId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public Term Term { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalMarks { get; set; } = 0;
        public decimal PassingMarks { get; set; } = 0;
        public ExamStatus Status { get; set; } = ExamStatus.Draft;
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedDate { get; set; }
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

        // Display properties (computed from navigation properties)
        public string? ClassName => Class?.Name;
        
        public string? SectionName => Section?.Name;

        
        public virtual ClassModel? Class { get; set; }

        [JsonIgnore]
        public virtual SectionModel? Section { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExamSubjectModel> ExamSubjects { get; set; } = new List<ExamSubjectModel>();

        [JsonIgnore]
        public virtual ICollection<ExamResultModel> ExamResults { get; set; } = new List<ExamResultModel>();

        [JsonIgnore]
        public virtual ICollection<ExamTimetableModel> ExamTimetables { get; set; } = new List<ExamTimetableModel>();
    }
}


