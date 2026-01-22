using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ExamTimetableRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? ExamId { get; set; }
        public DateTime? ExamDate { get; set; }
    }

    public class ExamTimetableCreateRequest
    {
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

        public int? InvigilatorId { get; set; }

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class ExamTimetableUpdateRequest
    {
        [Required]
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

        public int? InvigilatorId { get; set; }

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class BulkExamTimetableCreateRequest
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public List<ExamTimetableCreateRequest> Timetables { get; set; } = new List<ExamTimetableCreateRequest>();
    }
}


