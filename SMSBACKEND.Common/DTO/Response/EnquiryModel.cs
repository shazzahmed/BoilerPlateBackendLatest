using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class EnquiryModel : BaseClass
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string? FullName { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        public int ApplyingClass { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Source { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        // Related data
        public string? ClassName => Class?.Name;

        [ForeignKey("ApplyingClass")]
        public virtual ClassModel? Class { get; set; }
        
        // Workflow navigation for progress tracking
        public virtual PreAdmissionModel? PreAdmission { get; set; }
    }
}
