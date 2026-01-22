using System.ComponentModel.DataAnnotations;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class ExamMarksRequest
    {
        public PaginationParams PaginationParam { get; set; } = new PaginationParams();
        public int? ExamSubjectId { get; set; }
        public int? StudentId { get; set; }
        public MarksStatus? Status { get; set; }
    }

    public class ExamMarksCreateRequest
    {
        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TheoryMarks { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal PracticalMarks { get; set; } = 0;

        public bool IsAbsent { get; set; } = false;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public MarksStatus Status { get; set; } = MarksStatus.Draft;
    }

    public class ExamMarksUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TheoryMarks { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PracticalMarks { get; set; }

        public bool IsAbsent { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        [Required]
        public MarksStatus Status { get; set; }
    }

    public class BulkExamMarksCreateRequest
    {
        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public List<StudentMarksEntry> StudentMarks { get; set; } = new List<StudentMarksEntry>();
    }

    public class StudentMarksEntry
    {
        [Required]
        public int StudentId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TheoryMarks { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal PracticalMarks { get; set; } = 0;

        public bool IsAbsent { get; set; } = false;

        public string Remarks { get; set; } = string.Empty;
    }

    public class VerifyMarksRequest
    {
        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public List<int> MarksIds { get; set; } = new List<int>();

        public string VerifierRemarks { get; set; } = string.Empty;
    }

    public class ExamMarksImportRequest
    {
        [Required]
        public int ExamSubjectId { get; set; }

        [Required]
        public string FileData { get; set; } = string.Empty; // Base64 encoded Excel file

        public bool OverwriteExisting { get; set; } = false;
    }
}


