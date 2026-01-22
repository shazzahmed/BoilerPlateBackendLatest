using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace Common.DTO.Response
{
    public class AdmissionModel : BaseClass
    {
        public int Id { get; set; }
        public string AdmissionNumber { get; set; } = string.Empty;
        public int EntryTestId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        // Student tracking fields
        public bool IsStudentCreated { get; set; } = false;
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }

        // Display properties (mapped from navigation properties)
        public string? TestNumber { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }

        // Related workflow information
        public int? WorkflowId { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("EntryTestId")]
        [JsonIgnore] // Prevent circular reference (EntryTest already has Admission)
        public virtual EntryTestModel? EntryTest { get; set; }

        [ForeignKey("ClassId")]
        public virtual ClassModel? Class { get; set; }

        [ForeignKey("SectionId")]
        public virtual SectionModel? Section { get; set; }

        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }
    }
}