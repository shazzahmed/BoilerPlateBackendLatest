
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class TimetableModel : BaseClass
    {
        public int Id { get; set; }
        public int ClassSectionId { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public List<TimetableEntryModel> TimetableEntries { get; set; } = new();

        [ForeignKey("ClassSectionId")]
        [JsonIgnore]
        public virtual ClassSectionModel? ClassSection { get; set; }
    }

    public class TimetableEntryModel
    {
        public int Id { get; set; }
        public int TimetableId { get; set; }
        public int DayOfWeek { get; set; } // 1-6 (Monday-Saturday)
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? StartTime { get; set; } // Format: "08:00"
        public string? EndTime { get; set; } // Format: "08:45"
        public int Duration { get; set; } // in minutes
        public string? RoomNumber { get; set; }
        public bool IsBreak { get; set; }
        public string? BreakName { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("SubjectId")]
        public virtual SubjectModel? Subject { get; set; }

        [ForeignKey("TeacherId")]
        public virtual StaffModel? Teacher { get; set; }
    }
}
