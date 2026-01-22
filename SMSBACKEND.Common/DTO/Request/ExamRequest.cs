using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class ExamRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public ExamStatus? Status { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
    }

    public class ExamCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string ExamName { get; set; } = string.Empty;

        [Required]
        public ExamType ExamType { get; set; }

        [Required]
        public int ClassId { get; set; }

        public int? SectionId { get; set; }

        [Required]
        [MaxLength(10)]
        public string AcademicYear { get; set; } = string.Empty;

        [Required]
        public Term Term { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal TotalMarks { get; set; } = 0;

        public decimal PassingMarks { get; set; } = 0;

        public ExamStatus Status { get; set; } = ExamStatus.Draft;

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class ExamUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ExamName { get; set; } = string.Empty;

        [Required]
        public ExamType ExamType { get; set; }

        [Required]
        public int ClassId { get; set; }

        public int? SectionId { get; set; }

        [Required]
        [MaxLength(10)]
        public string AcademicYear { get; set; } = string.Empty;

        [Required]
        public Term Term { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public decimal TotalMarks { get; set; } = 0;

        public decimal PassingMarks { get; set; } = 0;

        [Required]
        public ExamStatus Status { get; set; }

        [MaxLength(1000)]
        public string Instructions { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class ExamPublishRequest
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public bool IsPublished { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}


