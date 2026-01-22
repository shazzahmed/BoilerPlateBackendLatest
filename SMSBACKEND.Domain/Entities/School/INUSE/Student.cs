using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class Student : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Navigation properties to normalized entities
        [JsonIgnore] // Prevent reference loops in caching
        public virtual StudentInfo? StudentInfo { get; set; }

        [JsonIgnore] // Prevent reference loops in caching
        public virtual StudentAddress? StudentAddress { get; set; }

        [JsonIgnore] // Prevent reference loops in caching
        public virtual StudentFinancial? StudentFinancial { get; set; }

        [JsonIgnore] // Prevent reference loops in caching
        public virtual StudentParent? StudentParent { get; set; }

        [JsonIgnore] // Prevent reference loops in caching
        public virtual StudentMiscellaneous? StudentMiscellaneous { get; set; }

        [JsonIgnore] // Prevent reference loops in caching
        public ICollection<StudentClassAssignment> ClassAssignments { get; set; } = new List<StudentClassAssignment>();

        // Simple computed properties for common access
        [NotMapped]
        public string FullName => $"{StudentInfo?.FirstName} {StudentInfo?.LastName}";

        [NotMapped]
        public string AdmissionNo => StudentInfo?.AdmissionNo ?? string.Empty;

        [NotMapped]
        public string FirstName => StudentInfo?.FirstName ?? string.Empty;

        [NotMapped]
        public string LastName => StudentInfo?.LastName ?? string.Empty;
    }
}
