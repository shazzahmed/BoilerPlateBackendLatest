using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ExamResultRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? ExamId { get; set; }
        public int? StudentId { get; set; }
        public string Result { get; set; } = string.Empty;
        public bool? IsPublished { get; set; }
    }

    public class GenerateExamResultRequest
    {
        [Required]
        public int ExamId { get; set; }

        public int? StudentId { get; set; } // If null, generate for all students

        public bool RecalculateIfExists { get; set; } = false;
    }

    public class PublishExamResultRequest
    {
        [Required]
        public int ExamId { get; set; }

        public List<int> StudentIds { get; set; } = new List<int>(); // If empty, publish all

        [Required]
        public bool IsPublished { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }

    public class ExamResultUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        public decimal? TotalMarksObtained { get; set; }

        public decimal? TotalMaxMarks { get; set; }

        public int? OverallGradeId { get; set; }

        public decimal? GPA { get; set; }

        public string Result { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class StudentReportCardRequest
    {
        [Required]
        public int ExamId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public int? TemplateId { get; set; } // Marksheet template
    }

    public class BulkReportCardRequest
    {
        [Required]
        public int ExamId { get; set; }

        public List<int> StudentIds { get; set; } = new List<int>(); // If empty, generate for all

        public int? TemplateId { get; set; }
    }
}


