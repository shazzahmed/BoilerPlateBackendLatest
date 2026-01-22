using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class StudentValidationRequest
    {
        [MaxLength(50)]
        public string AdmissionNo { get; set; } = string.Empty;

        [MaxLength(20)]
        public string RollNo { get; set; } = string.Empty;

        public int ClassId { get; set; }

        public int SectionId { get; set; }

        public int SessionId { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Optional: For updating existing student
        public int? ExistingStudentId { get; set; }
    }
}
