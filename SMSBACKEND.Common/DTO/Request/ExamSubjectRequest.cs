using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ExamSubjectRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? ExamId { get; set; }
        public int? SubjectId { get; set; }
    }

    public class ExamSubjectCreateRequest
    {
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
        public string ExamTime { get; set; } = string.Empty;

        public int Duration { get; set; } = 180;

        [MaxLength(50)]
        public string RoomNo { get; set; } = string.Empty;

        public bool IsOptional { get; set; } = false;

        [MaxLength(500)]
        public string Instructions { get; set; } = string.Empty;
    }

    public class ExamSubjectUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public decimal MaxMarks { get; set; }

        public decimal MaxPracticalMarks { get; set; } = 0;

        [Required]
        public decimal PassingMarks { get; set; }

        public decimal PassingPracticalMarks { get; set; } = 0;

        public DateTime? ExamDate { get; set; }

        [MaxLength(20)]
        public string ExamTime { get; set; } = string.Empty;

        public int Duration { get; set; }

        [MaxLength(50)]
        public string RoomNo { get; set; } = string.Empty;

        public bool IsOptional { get; set; }

        [MaxLength(500)]
        public string Instructions { get; set; } = string.Empty;
    }

    public class BulkExamSubjectCreateRequest
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public List<ExamSubjectCreateRequest> Subjects { get; set; } = new List<ExamSubjectCreateRequest>();
    }
}


