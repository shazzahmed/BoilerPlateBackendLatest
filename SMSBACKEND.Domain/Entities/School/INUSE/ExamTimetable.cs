using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class ExamTimetable : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [MaxLength(50)]
        public string RoomNo { get; set; } = string.Empty;

        public int? InvigilatorId { get; set; } // Staff/Teacher FK

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("ExamId")]
        [JsonIgnore]
        public virtual Exam Exam { get; set; }

        [ForeignKey("ExamSubjectId")]
        [JsonIgnore]
        public virtual ExamSubject ExamSubject { get; set; }

        [ForeignKey("InvigilatorId")]
        [JsonIgnore]
        public virtual Staff Invigilator { get; set; }
    }
}


