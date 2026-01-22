using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class EntryTestModel : BaseClass
    {
        public int Id { get; set; }
        public string? TestNumber { get; set; }
        public int ApplicationId { get; set; }
        public string? StudentName { get; set; }
        public DateTime? Dob { get; set; }
        public int ApplyingClass { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }
        public string? MotherName { get; set; }
        public string? MotherPhone { get; set; }
        public string? PreviousSchool { get; set; }
        public string? PreviousClass { get; set; }
        public DateTime TestDate { get; set; }
        public string? Status { get; set; }
        public int? TotalMarks { get; set; }
        public int? ObtainedMarks { get; set; }
        public decimal? Percentage { get; set; }
        public string? TestSubjects { get; set; }
        public string? TestResults { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Navigation properties for display
        public string? ApplicationNumber => Application?.ApplicationNumber;
        
        public string? ClassName => Class?.Name;

        [ForeignKey("ApplicationId")]
        [JsonIgnore] // Prevent circular reference (PreAdmission already has EntryTest)
        public virtual PreAdmissionModel? Application { get; set; }

        [ForeignKey("ApplyingClass")]
        public virtual ClassModel? Class { get; set; }
        
        // Reverse navigation for workflow tracking
        public virtual AdmissionModel? Admission { get; set; }
    }
}