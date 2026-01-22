
using Common.DTO.Request;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class StudentModel : BaseClass
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public required string AdmissionNo { get; set; }
        public DateTime? AdmissionDate { get; set; } =  DateTime.Now;
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public int SectionId { get; set; }
        public string? SectionName { get; set; }
        public int SessionId { get; set; }
        public string? SessionName { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime? Dob { get; set; }
        public required string Gender { get; set; }
        public string? Religion { get; set; }
        public string? Cast { get; set; }
        public string? Zipcode { get; set; }
        public string? PhoneNo { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? RollNo { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? CurrentAddress { get; set; }
        public string? AreaAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public int? SchoolHouseId { get; set; }
        public string? SchoolHouseName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public DateTime? MeasurementDate { get; set; }
        public string? StudentUserId { get; set; }
        public string? PreviousSchool { get; set; }
        public string? MedicalHistory { get; set; }
        public string? PickupPerson { get; set; }
        public string? Note { get; set; }
        public string? BloodGroup { get; set; }

        public string? BFormNumber { get; set; }
        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }
        public string? FatherOccupation { get; set; }
        public string? MotherCNIC { get; set; }
        public string? MotherName { get; set; }
        public string? MotherPhone { get; set; }
        public string? MotherOccupation { get; set; }
        public string? GuardianIs { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianRelation { get; set; }
        public string? GuardianEmail { get; set; }
        public string? GuardianPhone { get; set; }
        public string? GuardianOccupation { get; set; }
        public string? GuardianAddress { get; set; }
        public string? GuardianAreaAddress { get; set; }
        public string? FatherPic { get; set; }
        public string? MotherPic { get; set; }
        public string? GuardianPic { get; set; }

        public float? AdmissionFees { get; set; }
        public float? SecurityDeposit { get; set; }
        public decimal MonthlyFees { get; set; }
        public decimal Arrears { get; set; }
        public string? ParentId { get; set; }
        public string? FatherCNIC { get; set; }

        //public string Rte { get; set; }
        //public int RouteId { get; set; }
        //public int VehRouteId { get; set; }
        //public int HostelRoomId { get; set; }
        //public string AppKey { get; set; }
        //public string ParentAppKey { get; set; }
        //public string BankAccountNo { get; set; }
        //public string BankName { get; set; }
        //public string IfscCode { get; set; }

        public int DisReason { get; set; }
        public string? DisNote { get; set; }
        public DateTime? DisableAt { get; set; }
        [JsonIgnore]
        public ApplicationUserModel? Parent { get; set; }
        [JsonIgnore]
        public ApplicationUserModel? StudentUser { get; set; }
        // ✅ Removed [JsonIgnore] to allow siblings to be serialized in API response
        public List<StudentModel>? Siblings { get; set; }
        [ForeignKey("CategoryId")]
        public StudentCategoryModel? StudentCategory { get; set; }

        [ForeignKey("SchoolHouseId")]
        public SchoolHouseModel? SchoolHouse { get; set; }
        [JsonIgnore]
        public ICollection<StudentClassAssignmentModel>? ClassAssignments { get; set; }
        
        // ✅ Additional data for detailed view
        public object? StudentUserAccount { get; set; }
        public object? ParentUserAccount { get; set; }
        public object? AttendanceSummary { get; set; }
        public object? FeeSummary { get; set; }
        public List<ExamResultModel>? RecentExamResults { get; set; }
        
        // Note: This model now uses flat structure instead of nested classes
    }
}
