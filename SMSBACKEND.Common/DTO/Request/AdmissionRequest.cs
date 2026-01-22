using System.ComponentModel.DataAnnotations;
using Common.DTO.Response;

namespace Common.DTO.Request
{
    public class AdmissionRequest
    {
        public int Id { get; set; }
        public string AdmissionNumber { get; set; } = string.Empty;
        public int EntryTestId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Status { get; set; } = "Active";
        public string? Remarks { get; set; }
    }

    public class AdmissionCreateRequest
    {
        [Required]
        public int EntryTestId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        public string Status { get; set; } = "Active";

        public string? Remarks { get; set; }
    }

    public class AdmissionUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int EntryTestId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}
