
using Common.DTO.Request;
using System.Security.Claims;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class StudentClassAssignmentModel : BaseClass
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int SessionId { get; set; }

        // Additional Details
        public DateTime AssignedDate { get; set; } // When the assignment was made
        public string? Remarks { get; set; }        // Optional remarks

        // Navigation Properties
        public StudentModel? Student { get; set; }
        public ClassModel? Class { get; set; }
        public SectionModel? Section { get; set; }
    }
}
