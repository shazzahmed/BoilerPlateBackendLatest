using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class Enquiry : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

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
        public string? Source { get; set; } // 'Website', 'Referral', 'Walk-in', 'Advertisement', 'Other'

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "New"; // 'New', 'Contacted', 'Interested', 'NotInterested', 'Converted', 'Recommended', etc.

        // Navigation properties
        [ForeignKey("ApplyingClass")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [JsonIgnore]
        public virtual PreAdmission? PreAdmission { get; set; }
    }
}
