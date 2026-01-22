using Common.DTO.Response;
using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class EntryTestRequest : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string TestNumber { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [Required]
        [MaxLength(200)]
        public string StudentName { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        public int ApplyingClass { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(200)]
        public string FatherName { get; set; }

        [MaxLength(20)]
        public string FatherPhone { get; set; }

        [MaxLength(200)]
        public string MotherName { get; set; }

        [MaxLength(20)]
        public string MotherPhone { get; set; }

        [MaxLength(1000)]
        public string PreviousSchool { get; set; }

        [MaxLength(1000)]
        public string PreviousClass { get; set; }

        public DateTime TestDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        public int? TotalMarks { get; set; }

        public int? ObtainedMarks { get; set; }

        public decimal? Percentage { get; set; }

        [MaxLength(1000)]
        public string TestSubjects { get; set; }

        [MaxLength(1000)]
        public string TestResults { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public DateTime? CompletedDate { get; set; }
    }

    public class EntryTestCreateRequest : EntryTestRequest
    {
        // Inherits all properties from EntryTestRequest
    }

    public class EntryTestUpdateRequest : EntryTestRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
