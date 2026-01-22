
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class StudentAttendanceModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }
        public int SessionId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public AttendanceType AttendanceType { get; set; }
        public string Remark { get; set; }
        public bool IsBiometricAttendance { get; set; }
        public string BiometricDeviceData { get; set; }

        [ForeignKey("ClassId")]
        public virtual ClassModel Class { get; set; }

        [ForeignKey("SectionId")]
        public virtual SectionModel Section { get; set; }

        [ForeignKey("StudentId")]
        public virtual StudentModel Student { get; set; }
        [JsonIgnore]
        public List<StudentAttendanceModel> MonthlyAttendance { get; set; }
    }
}
