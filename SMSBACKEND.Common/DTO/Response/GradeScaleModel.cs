using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class GradeScaleModel
    {
        public int Id { get; set; }
        public string GradeScaleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

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
        public virtual ICollection<GradeModel> Grades { get; set; } = new List<GradeModel>();
    }
}


