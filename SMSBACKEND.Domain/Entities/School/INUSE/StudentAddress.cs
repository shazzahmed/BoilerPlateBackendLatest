using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class StudentAddress : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(500)]
        public string? CurrentAddress { get; set; }

        [MaxLength(500)]
        public string? AreaAddress { get; set; }

        [MaxLength(500)]
        public string? PermanentAddress { get; set; }

        [MaxLength(20)]
        public string? Zipcode { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;
    }
}