using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class GradeModel
    {
        public int Id { get; set; }
        public int GradeScaleId { get; set; }
        public string GradeName { get; set; } = string.Empty;
        public decimal MinPercentage { get; set; } = 0;
        public decimal MaxPercentage { get; set; } = 0;
        public decimal GradePoint { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "#000000";
        public int DisplayOrder { get; set; } = 0;

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
        public virtual GradeScaleModel GradeScale { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExamMarksModel> ExamMarks { get; set; } = new List<ExamMarksModel>();

        [JsonIgnore]
        public virtual ICollection<ExamResultModel> ExamResults { get; set; } = new List<ExamResultModel>();
    }
}


