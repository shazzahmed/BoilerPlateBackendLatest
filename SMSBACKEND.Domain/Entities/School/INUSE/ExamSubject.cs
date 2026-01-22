using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ExamSubject : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public decimal MaxMarks { get; set; } = 100;

        public decimal MaxPracticalMarks { get; set; } = 0;

        [Required]
        public decimal PassingMarks { get; set; } = 33;

        public decimal PassingPracticalMarks { get; set; } = 0;

        public DateTime? ExamDate { get; set; }

        [MaxLength(20)]
        public string? ExamTime { get; set; } // e.g., "09:00 AM"

        public int Duration { get; set; } = 180; // Duration in minutes

        [MaxLength(50)]
        public string? RoomNo { get; set; }

        public bool IsOptional { get; set; } = false;

        [MaxLength(500)]
        public string? Instructions { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        [JsonIgnore]
        public virtual Exam Exam { get; set; } = null!;

        [ForeignKey("SubjectId")]
        [JsonIgnore]
        public virtual Subject Subject { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<ExamMarks> ExamMarks { get; set; } = new List<ExamMarks>();

        [JsonIgnore]
        public virtual ExamTimetable? ExamTimetable { get; set; }
    }
}


