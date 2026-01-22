using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class TimetableDifferentialUpdateRequest
    {
        [Required]
        public int ClassSectionId { get; set; }
        public List<TimetableEntryDifferentialRequest> EntriesToAdd { get; set; } = new();
        public List<TimetableEntryDifferentialRequest> EntriesToUpdate { get; set; } = new();
        public List<int> EntriesToDelete { get; set; } = new(); // Entry IDs to delete
    }

    public class TimetableEntryDifferentialRequest
    {
        public int? Id { get; set; } // null for new entries, existing ID for updates
        [Required]
        public int DayOfWeek { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
        [Required]
        public required string StartTime { get; set; }
        [Required]
        public required string EndTime { get; set; }
        [Required]
        public int Duration { get; set; }
        public required string RoomNumber { get; set; }
        public bool IsBreak { get; set; }
        public string? BreakName { get; set; }
        [Required]
        public int OrderIndex { get; set; }
    }
}
