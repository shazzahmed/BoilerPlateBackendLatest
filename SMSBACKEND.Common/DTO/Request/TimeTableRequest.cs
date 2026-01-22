using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class TimetableCreateRequest
    {
        [Required]
        public int ClassSectionId { get; set; }
        public List<TimetableEntryCreateRequest> TimetableEntries { get; set; } = new();
    }

    public class TimetableEntryCreateRequest
    {
        [Required]
        public int DayOfWeek { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string EndTime { get; set; }
        [Required]
        public int Duration { get; set; }
        public string RoomNumber { get; set; }
        public bool IsBreak { get; set; }
        public string BreakName { get; set; }
        [Required]
        public int OrderIndex { get; set; }
    }

    public class TimetableUpdateRequest : TimetableCreateRequest
    {
        [Required]
        public int Id { get; set; }
    }

    public class TimetableExportRequest
    {
        [Required]
        public int ClassSectionId { get; set; }
        [Required]
        public string Format { get; set; } // "pdf" or "excel"
    }
}