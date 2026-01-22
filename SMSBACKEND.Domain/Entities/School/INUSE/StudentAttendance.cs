using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents daily attendance record for a student
    /// </summary>
    public class StudentAttendance : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public AttendanceType AttendanceType { get; set; }

        [MaxLength(500)]
        public string? Remark { get; set; }

        public bool IsBiometricAttendance { get; set; } = false;

        [MaxLength(1000)]
        public string? BiometricDeviceData { get; set; }

        // Navigation properties
        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("SectionId")]
        [JsonIgnore]
        public virtual Section Section { get; set; } = null!;

        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey("SessionId")]
        [JsonIgnore]
        public virtual Session Session { get; set; } = null!;
    }
}
